// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Raycynix.Extensions.Database.AspNetCore.Identity;
using Raycynix.Extensions.Exceptions;
using Raycynix.Extensions.Security.Abstractions.Interfaces;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Application.Extensions;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Application.Services;

/// <summary>
/// Provides authentication operations backed by ASP.NET Core Identity and refresh-token persistence.
/// </summary>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
/// <param name="signInManager">The ASP.NET Core Identity sign-in manager.</param>
/// <param name="jwtSettings">The JWT configuration options.</param>
/// <param name="secretResolver">The secret resolver used to read the JWT signing secret.</param>
/// <param name="databaseContext">The Identity database context used to persist refresh tokens.</param>
/// <param name="logger">The logger used to write authentication events.</param>
public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JwtConfiguration> jwtSettings,
    ISecretResolver secretResolver,
    RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
        databaseContext,
    Raycynix.Extensions.Logging.Abstractions.ILogger<AuthService> logger) : IAuthService
{
    /// <inheritdoc />
    public async Task<AuthResult> RegisterAsync(RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = new User(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError(
                "User trying to register with username:{UserName} and email:{Email}.\nGet`s errors:{Errors}",
                request.UserName, request.Email, errors);

            throw new ConflictException(errors);
        }

        var accessToken = await user.GenerateTokenAsync(secretResolver, jwtSettings.Value);
        var refreshToken = SecurityExtensions.GenerateRefreshToken();

        var userRefreshToken = user.GenerateUserRefreshToken(refreshToken, jwtSettings.Value);
        await databaseContext.AddAsync(userRefreshToken, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken.TokenString(), accessToken.ValidTo, refreshToken);
    }

    /// <inheritdoc />
    public async Task<AuthResult> LoginAsync(LoginRequest request, string? refreshToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var login = request.Login.Trim();

        var user = login.Contains('@')
            ? await userManager.FindByEmailAsync(login)
            : await userManager.FindByNameAsync(login);
        if (user is null)
        {
            logger.LogError("Login Failed: user with login:{login} not found", login);
            throw new UnauthorizedException("Invalid login or password");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            logger.LogWarning("Login Failed: invalid Password for user:{userId}", user.Id);
            throw new UnauthorizedException("Invalid login or password");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.UpdateAsync(user);

        var accessToken = await user.GenerateTokenAsync(secretResolver, jwtSettings.Value);
        var newRefreshToken = SecurityExtensions.GenerateRefreshToken();

        var newUserRefreshToken = user.GenerateUserRefreshToken(newRefreshToken, jwtSettings.Value);

        if (refreshToken is not null)
        {
            logger.LogInformation("Refresh token is not null. Checking for old refresh token");
            var refreshTokenHash = SecurityExtensions.HashRefreshToken(refreshToken);

            var oldUserRefreshToken = await databaseContext.Set<UserRefreshToken>()
                .FirstOrDefaultAsync(
                    token => token.UserId == user.Id &&
                             token.TokenHash == refreshTokenHash &&
                             token.RevokedAt == null &&
                             token.ExpiresAt > DateTimeOffset.UtcNow,
                    cancellationToken);
            if (oldUserRefreshToken is not null)
            {
                logger.LogInformation("Old refresh token found. Revoking it");
                oldUserRefreshToken.RevokedAt = DateTimeOffset.UtcNow;
                oldUserRefreshToken.ReplacedByTokenHash = newUserRefreshToken.TokenHash;
                databaseContext.Update(oldUserRefreshToken);
            }
        }

        await databaseContext.AddAsync(newUserRefreshToken, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken.TokenString(), accessToken.ValidTo, newRefreshToken);
    }

    /// <inheritdoc />
    public async Task LogoutAsync(string? refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogWarning("Logout failed: refresh token cookie is missing");
            return;
        }

        var tokenHash = SecurityExtensions.HashRefreshToken(refreshToken);

        var userRefreshToken = await databaseContext.Set<UserRefreshToken>()
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash &&
                         token.RevokedAt == null &&
                         token.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
        if (userRefreshToken is null)
        {
            logger.LogWarning("Logout failed: refresh token not found");
            return;
        }

        userRefreshToken.RevokedAt = DateTimeOffset.UtcNow;

        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthResult> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogWarning("Refresh token failed: refresh token is null or empty");
            throw new UnauthorizedException("Refresh token is null or empty");
        }

        var tokenHash = SecurityExtensions.HashRefreshToken(refreshToken);
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            logger.LogWarning("Refresh token failed: refresh token hash is null or empty");
            throw new UnauthorizedException("Refresh token hash is null or empty");
        }

        var userRefreshToken = await databaseContext.Set<UserRefreshToken>()
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash &&
                         token.RevokedAt == null &&
                         token.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
        if (userRefreshToken is null)
        {
            logger.LogWarning("Refresh token failed: refresh token not found");
            throw new UnauthorizedException("Refresh token not found");
        }

        var user = await databaseContext.Set<User>()
            .SingleOrDefaultAsync(u => u.Id == userRefreshToken.UserId, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Refresh token failed: user by id:{userId} not found in database",
                userRefreshToken.UserId);
            throw new UnauthorizedException("Refresh token not found");
        }

        var accessToken = await user.GenerateTokenAsync(secretResolver, jwtSettings.Value);
        var newRefreshToken = SecurityExtensions.GenerateRefreshToken();

        var newUserRefreshToken = user.GenerateUserRefreshToken(newRefreshToken, jwtSettings.Value);
        await databaseContext.AddAsync(newUserRefreshToken, cancellationToken);

        userRefreshToken.RevokedAt = DateTimeOffset.UtcNow;
        userRefreshToken.ReplacedByTokenHash = newUserRefreshToken.TokenHash;
        databaseContext.Update(userRefreshToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken.TokenString(), accessToken.ValidTo, newRefreshToken);
    }
}
