// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Identity.Host.Application.Interfaces;
using Raycynix.Identity.Host.Application.Models;
using Raycynix.Identity.Host.Web.Extensions;

namespace Raycynix.Identity.Host.Web.Controllers;

/// <summary>
/// Handles authentication HTTP endpoints.
/// </summary>
/// <param name="authService">The authentication service.</param>
/// <param name="jwtSettings">The JWT configuration options.</param>
[ApiController]
[Route(AuthRoutes.Base)]
public class AuthController(
    IAuthService authService,
    IOptions<JwtConfiguration> jwtSettings
) : ControllerBase
{
    private const string AuthSensitiveRateLimitPolicy = "auth-sensitive";

    /// <summary>
    /// Registers a new user and sends an email confirmation link.
    /// </summary>
    /// <param name="request">The registration data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The registration response.</returns>
    [HttpPost(AuthRoutes.Registration)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        return Ok(new RegisterResponse(result.Email));
    }

    /// <summary>
    /// Authenticates a user and sets the issued refresh token in an HTTP-only cookie.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The access token response.</returns>
    [HttpPost(AuthRoutes.Login)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies.GetRefreshToken();

        var result = await authService.LoginAsync(request, refreshToken, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);

        return Ok(new AuthResponse(
                result.AccessToken,
                result.AccessTokenExpires
            )
        );
    }

    /// <summary>
    /// Sends a new email confirmation link.
    /// </summary>
    /// <param name="request">The email confirmation token request.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when the email confirmation link is sent.</returns>
    [HttpPost(AuthRoutes.EmailConfirmationSend)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SendEmailConfirmationLinkAsync(
        [FromBody] EmailConfirmationLinkRequest request,
        CancellationToken cancellationToken)
    {
        await authService.SendEmailConfirmationLinkAsync(request, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <param name="request">The email confirmation request.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when the email address is confirmed.</returns>
    [HttpPost(AuthRoutes.EmailConfirmationConfirm)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        await authService.ConfirmEmailAsync(request, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Confirms a user's email address from an email confirmation link.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="token">The encoded email confirmation token.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when the email address is confirmed.</returns>
    [HttpGet(AuthRoutes.EmailConfirmationConfirm)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string email, [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        await authService.ConfirmEmailAsync(new ConfirmEmailRequest(email, token), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Sends a password reset link.
    /// </summary>
    /// <param name="request">The password reset link request.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when the password reset link is sent or skipped.</returns>
    [HttpPost(AuthRoutes.PasswordResetSend)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SendPasswordResetLinkAsync([FromBody] PasswordResetLinkRequest request,
        CancellationToken cancellationToken)
    {
        await authService.SendPasswordResetLinkAsync(request, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    /// <param name="request">The password reset request.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when the password is reset.</returns>
    [HttpPost(AuthRoutes.PasswordReset)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await authService.ResetPasswordAsync(request, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Revokes the current refresh token and removes its cookie.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when logout processing is complete.</returns>
    [HttpPost(AuthRoutes.Logout)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies.GetRefreshToken();

        await authService.LogoutAsync(refreshToken, cancellationToken);

        Response.Cookies.DeleteRefreshToken();

        return NoContent();
    }

    /// <summary>
    /// Rotates the current refresh token and returns a new access token.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The refreshed access token response.</returns>
    [HttpPost(AuthRoutes.Refresh)]
    [EnableRateLimiting(AuthSensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies.GetRefreshToken();

        var result = await authService.RefreshTokenAsync(refreshToken, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);

        return Ok(new AuthResponse(result.AccessToken, result.AccessTokenExpires));
    }
}
