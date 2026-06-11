// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Application.Interfaces;

/// <summary>
/// Defines authentication operations for registration, login, logout, and token refresh.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and issues a new access token and refresh token pair.
    /// </summary>
    /// <param name="request">The registration data used to create the user.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The issued authentication tokens.</returns>
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates an existing user by username or email, optionally revokes the current refresh token, and issues a new token pair.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <param name="refreshToken">The current raw refresh token received from the client, if any.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The issued authentication tokens.</returns>
    Task<AuthResult> LoginAsync(LoginRequest request, string? refreshToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token if it exists.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token received from the client.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates an active refresh token and issues a new access token and refresh token pair.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token received from the client.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The refreshed authentication tokens.</returns>
    Task<AuthResult> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default);
}
