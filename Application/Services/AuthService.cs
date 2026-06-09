// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JwtConfiguration> jwtSettings,
    ISecretResolver secretResolver,
    RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> databaseContext,
    Raycynix.Extensions.Logging.Abstractions.ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogError("Register Failed: username, email or password is null or empty");
            throw new ArgumentNullException();
        }
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

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogError("Login Failed: login or password is null or empty");
            throw new ArgumentNullException();
        }
        var login = request.Login.Trim();

        var user = login.Contains('@')
            ? await userManager.FindByEmailAsync(login)
            : await userManager.FindByNameAsync(login);
        if (user is null)
        {
            logger.LogError("Login Failed: user with login:{login} not found", login);
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            logger.LogWarning("Login Failed: invalid Password for user:{userId}", user.Id);
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.UpdateAsync(user);

        var accessToken = await user.GenerateTokenAsync(secretResolver, jwtSettings.Value);
        var refreshToken = SecurityExtensions.GenerateRefreshToken();

        var userRefreshToken = user.GenerateUserRefreshToken(refreshToken, jwtSettings.Value);
        await databaseContext.AddAsync(userRefreshToken, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken.TokenString(), accessToken.ValidTo, refreshToken);
    }

    public async Task LogoutAsync(ClaimsPrincipal userClaims, string? refreshToken, CancellationToken cancellationToken = default)
    {
        var userIdValue = userClaims.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? userClaims.FindFirstValue(ClaimTypes.Name);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            logger.LogWarning("Logout failed: user ID not found in claims.");
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogWarning("Logout failed: refresh token cookie is missing for user:{UserId}", userId);
            return;
        }

        var tokenHash = SecurityExtensions.HashRefreshToken(refreshToken);

        var userRefreshToken = await databaseContext.Set<UserRefreshToken>()
            .FirstOrDefaultAsync(
                token => token.UserId == userId &&
                         token.TokenHash == tokenHash &&
                         token.RevokedAt == null,
                cancellationToken);

        if (userRefreshToken is null)
        {
            logger.LogWarning("Logout failed: refresh token not found for user:{UserId}", userId);
            return;
        }

        userRefreshToken.RevokedAt = DateTime.UtcNow;

        await databaseContext.SaveChangesAsync(cancellationToken);
    }
}
