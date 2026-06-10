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

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IAuthService authService,
    IOptions<JwtConfiguration> jwtSettings
) : ControllerBase
{
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);

        return Ok(new AuthResponse(
                result.AccessToken,
                result.AccessTokenExpires
            )
        );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieExtensions.RefreshTokenCookieName];

        await authService.LogoutAsync(refreshToken, cancellationToken);

        Response.Cookies.DeleteRefreshToken();

        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieExtensions.RefreshTokenCookieName];

        var result = await authService.RefreshTokenAsync(refreshToken, cancellationToken);

        Response.Cookies.AppendRefreshToken(result.RefreshToken, jwtSettings.Value.RefreshTokenLifetime);
        
        return Ok(new AuthResponse(result.AccessToken, result.AccessTokenExpires));
    }
}
