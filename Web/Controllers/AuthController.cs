// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Web.Extensions;

namespace Raycynix.Services.AuthService.Web.Controllers;

/// <summary>
/// Handles authentication HTTP endpoints.
/// </summary>
/// <param name="authService">The authentication service.</param>
/// <param name="jwtSettings">The JWT configuration options.</param>
[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IAuthService authService,
    IOptions<JwtConfiguration> jwtSettings
) : ControllerBase
{
    /// <summary>
    /// Registers a new user and sets the issued refresh token in an HTTP-only cookie.
    /// </summary>
    /// <param name="request">The registration data.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The access token response.</returns>
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);

        return Ok(new AuthResponse(
                result.AccessToken,
                result.AccessTokenExpires
            )
        );
    }

    /// <summary>
    /// Authenticates a user and sets the issued refresh token in an HTTP-only cookie.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The access token response.</returns>
    [HttpPost("login")]
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
    /// Revokes the current refresh token and removes its cookie.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>An empty response when logout processing is complete.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieExtensions.RefreshTokenCookieName];

        await authService.LogoutAsync(refreshToken, cancellationToken);

        Response.Cookies.DeleteRefreshToken();

        return NoContent();
    }

    /// <summary>
    /// Rotates the current refresh token and returns a new access token.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The refreshed access token response.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieExtensions.RefreshTokenCookieName];

        var result = await authService.RefreshTokenAsync(refreshToken, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);
        
        return Ok(new AuthResponse(result.AccessToken, result.AccessTokenExpires));
    }
}
