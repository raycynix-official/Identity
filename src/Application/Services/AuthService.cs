// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Raycynix.Extensions.Database.AspNetCore.Identity;
using Raycynix.Extensions.Email.Abstractions.Exceptions;
using Raycynix.Extensions.Email.Abstractions.Interfaces;
using Raycynix.Extensions.Email.Abstractions.Models;
using Raycynix.Extensions.Exceptions;
using Raycynix.Extensions.Security.Abstractions.Interfaces;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Application.Extensions;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Domain.Configurations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;
using System.Net;
using System.Text;

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
/// <param name="identityOptions">The ASP.NET Core Identity behavior configuration options.</param>
/// <param name="emailSender">The email sender used to deliver account emails.</param>
/// <param name="emailConfirmationConfiguration">The email confirmation delivery configuration options.</param>
/// <param name="resetPasswordConfiguration">The reset-password email delivery configuration options.</param>
public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JwtConfiguration> jwtSettings,
    ISecretResolver secretResolver,
    RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
        databaseContext,
    Raycynix.Extensions.Logging.Abstractions.ILogger<AuthService> logger,
    IOptions<IdentityOptions> identityOptions,
    IOptions<EmailConfirmationConfiguration> emailConfirmationConfiguration,
    IOptions<ResetPasswordConfiguration> resetPasswordConfiguration,
    IEmailSender emailSender) : IAuthService
{
    /// <inheritdoc />
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = new User(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorCodes = string.Join(", ", result.Errors.Select(e => e.Code));
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError("User registration failed. ErrorCodes:{ErrorCodes}", errorCodes);

            throw new ConflictException(errors);
        }

        var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendEmailConfirmationAsync(user, emailConfirmationToken, cancellationToken);

        return new RegisterResult(user.Email!);
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
            logger.LogError("Login failed: user was not found");
            throw new UnauthorizedException("Invalid login or password");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
        {
            if (result.IsNotAllowed &&
                identityOptions.Value.SignIn.RequireConfirmedEmail &&
                !await userManager.IsEmailConfirmedAsync(user))
            {
                logger.LogWarning("Login Failed: email is not confirmed for user:{userId}", user.Id);
                throw new UnauthorizedException("Email is not confirmed");
            }

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
    public async Task SendEmailConfirmationLinkAsync(EmailConfirmationLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            logger.LogWarning("Email confirmation token generation failed: user was not found");
            throw new UnauthorizedException("User not found");
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("Email confirmation token generation failed: email already confirmed for user:{userId}",
                user.Id);
            throw new UnauthorizedException("Email already confirmed");
        }

        var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendEmailConfirmationAsync(user, emailConfirmationToken, cancellationToken);
    }


    /// <inheritdoc />
    public async Task SendPasswordResetLinkAsync(PasswordResetLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            logger.LogWarning("Password reset link generation skipped: user was not found");
            return;
        }

        if (identityOptions.Value.SignIn.RequireConfirmedEmail && !await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("Password reset link generation skipped: email is not confirmed for user:{userId}",
                user.Id);
            return;
        }

        var resetPasswordToken = await userManager.GeneratePasswordResetTokenAsync(user);
        await SendPasswordResetAsync(user, resetPasswordToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            logger.LogWarning("Email confirmation failed: user was not found");
            throw new UnauthorizedException("User not found");
        }

        var result = await userManager.ConfirmEmailAsync(user, DecodeIdentityToken(request.Token));
        if (result.Succeeded)
        {
            return;
        }

        var errorCodes = string.Join(", ", result.Errors.Select(e => e.Code));
        logger.LogWarning("Email confirmation failed for user:{userId}. ErrorCodes:{errorCodes}", user.Id, errorCodes);
        throw new UnauthorizedException("Invalid email confirmation token");
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            logger.LogWarning("Password reset failed: user was not found");
            throw new UnauthorizedException("User not found");
        }

        var result = await userManager.ResetPasswordAsync(user, DecodeIdentityToken(request.Token), request.NewPassword);
        if (result.Succeeded)
        {
            await RevokeRefreshTokensAsync(user.Id, cancellationToken);
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        var errorCodes = string.Join(", ", result.Errors.Select(e => e.Code));
        logger.LogWarning("Password reset failed for user:{userId}. ErrorCodes:{errorCodes}", user.Id, errorCodes);
        throw new ConflictException(errors);
    }

    /// <inheritdoc />
    public async Task LogoutAsync(string? refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogWarning("Logout failed: refresh token cookie is missing");
            throw new UnauthorizedException("Refresh token cookie is missing");
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
            throw new UnauthorizedException("Refresh token not found");
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

    private async Task RevokeRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeRefreshTokens = await databaseContext.Set<UserRefreshToken>()
            .Where(token => token.UserId == userId &&
                            token.RevokedAt == null &&
                            token.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

        if (activeRefreshTokens.Count == 0)
        {
            return;
        }

        var revokedAt = DateTimeOffset.UtcNow;
        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.RevokedAt = revokedAt;
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SendEmailConfirmationAsync(User user, string confirmationToken,
        CancellationToken cancellationToken)
    {
        var confirmationLink = CreateEmailConfirmationLink(user.Email!, confirmationToken);
        var htmlBody = await RenderEmailConfirmationTemplateAsync(
            user,
            confirmationLink,
            cancellationToken);

        var message = new EmailMessage
        {
            To = [new EmailAddress(user.Email!, user.UserName ?? user.Email!)],
            Subject = "Confirm your Raycynix account",
            Body = EmailBody.FromHtml(
                htmlBody,
                $"""
                 Confirm your Raycynix account email.

                 {confirmationLink}

                 Open this link to finish account setup.
                 """)
        };

        var result = await emailSender.SendAsync(message, cancellationToken);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Email confirmation message failed for user:{userId}. Provider:{provider}. ErrorCode:{errorCode}",
                user.Id, result.Provider, result.ErrorCode);
            throw new EmailSendException(
                $"Email confirmation message could not be sent by provider '{result.Provider}'. ErrorCode: {result.ErrorCode}");
        }

        logger.LogInformation("Email confirmation message sent for user:{userId}. Provider:{provider}",
            user.Id, result.Provider);
    }

    private async Task SendPasswordResetAsync(User user, string resetPasswordToken,
        CancellationToken cancellationToken)
    {
        var resetPasswordLink = CreatePasswordResetLink(user.Email!, resetPasswordToken);
        var htmlBody = await RenderResetPasswordTemplateAsync(
            user,
            resetPasswordLink,
            cancellationToken);

        var message = new EmailMessage
        {
            To = [new EmailAddress(user.Email!, user.UserName ?? user.Email!)],
            Subject = "Reset your Raycynix account password",
            Body = EmailBody.FromHtml(
                htmlBody,
                $"""
                 To reset your password, please click the link below:

                 {resetPasswordLink}

                 Open this link to reset your password.
                 """)
        };

        var result = await emailSender.SendAsync(message, cancellationToken);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Reset password message failed for user:{userId}. Provider:{provider}. ErrorCode:{errorCode}",
                user.Id, result.Provider, result.ErrorCode);
            throw new EmailSendException(
                $"Reset password message could not be sent by provider '{result.Provider}'. ErrorCode: {result.ErrorCode}");
        }

        logger.LogInformation("Reset password message sent for user:{userId}. Provider:{provider}",
            user.Id, result.Provider);
    }

    private async Task<string> RenderEmailConfirmationTemplateAsync(User user, string confirmationLink,
        CancellationToken cancellationToken)
    {
        var templatePath = ResolveTemplatePath(emailConfirmationConfiguration.Value.TemplatePath);
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        return template
            .Replace("{{UserName}}", WebUtility.HtmlEncode(user.UserName ?? user.Email), StringComparison.Ordinal)
            .Replace("{{Email}}", WebUtility.HtmlEncode(user.Email), StringComparison.Ordinal)
            .Replace("{{ConfirmationLink}}", WebUtility.HtmlEncode(confirmationLink), StringComparison.Ordinal);
    }

    private async Task<string> RenderResetPasswordTemplateAsync(User user, string resetPasswordLink,
        CancellationToken cancellationToken)
    {
        var templatePath = ResolveTemplatePath(resetPasswordConfiguration.Value.TemplatePath);
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        return template
            .Replace("{{UserName}}", WebUtility.HtmlEncode(user.UserName ?? user.Email), StringComparison.Ordinal)
            .Replace("{{ResetPasswordLink}}", WebUtility.HtmlEncode(resetPasswordLink), StringComparison.Ordinal);
    }

    private static string ResolveTemplatePath(string templatePath)
    {
        return Path.IsPathRooted(templatePath)
            ? templatePath
            : Path.Combine(AppContext.BaseDirectory, templatePath);
    }

    private string CreateEmailConfirmationLink(string email, string confirmationToken)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
        return QueryHelpers.AddQueryString(
            CreateEndpointUrl(AuthRoutes.EmailConfirmationConfirmPath),
            new Dictionary<string, string?>
            {
                [nameof(ConfirmEmailRequest.Email).ToLowerInvariant()] = email,
                [nameof(ConfirmEmailRequest.Token).ToLowerInvariant()] = encodedToken
            });
    }

    private string CreatePasswordResetLink(string email, string resetPasswordToken)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));
        return QueryHelpers.AddQueryString(
            CreateEndpointUrl(AuthRoutes.PasswordResetPath),
            new Dictionary<string, string?>
            {
                [nameof(ResetPasswordRequest.Email).ToLowerInvariant()] = email,
                [nameof(ResetPasswordRequest.Token).ToLowerInvariant()] = encodedToken
            });
    }

    private string CreateEndpointUrl(string route)
    {
        var authority = jwtSettings.Value.Authority;
        if (string.IsNullOrWhiteSpace(authority))
        {
            throw new InvalidOperationException("JWT authority is required to build account email links");
        }

        return $"{authority.TrimEnd('/')}/{route}";
    }

    private static string DecodeIdentityToken(string token)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return token;
        }
    }
}
