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
    /// Registers a new user and issues an access token and refresh token.
    /// </summary>
    /// <param name="request">The registration data used to create the user.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The issued authentication tokens.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="Raycynix.Extensions.Exceptions.ConflictException">
    /// Thrown when Identity rejects the registration request.
    /// </exception>
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user by user name or email, optionally revokes the supplied refresh token, and issues a new token pair.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <param name="refreshToken">The current raw refresh token received from the client, or <see langword="null"/> when none was supplied.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The issued authentication tokens.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="Raycynix.Extensions.Exceptions.UnauthorizedException">
    /// Thrown when the login value does not match a user or the password check fails.
    /// </exception>
    Task<AuthResult> LoginAsync(LoginRequest request, string? refreshToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token if it exists; missing, blank, expired, or already revoked tokens are ignored.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token received from the client, or <see langword="null"/> when none was supplied.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token and issues a replacement access token and refresh token.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token received from the client, or <see langword="null"/> when none was supplied.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The refreshed authentication tokens.</returns>
    /// <exception cref="Raycynix.Extensions.Exceptions.UnauthorizedException">
    /// Thrown when the refresh token is missing, invalid, expired, revoked, or no longer belongs to an existing user.
    /// </exception>
    Task<AuthResult> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default);
}
