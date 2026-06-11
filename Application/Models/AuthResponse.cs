// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Application.Models;

/// <summary>
/// Represents the authentication data returned to API clients.
/// </summary>
/// <param name="AccessToken">The issued access token.</param>
/// <param name="AccessTokenExpires">The UTC expiration date of the access token.</param>
public record AuthResponse(string AccessToken, DateTime AccessTokenExpires);

/// <summary>
/// Represents the full authentication result used inside the service.
/// </summary>
/// <param name="AccessToken">The issued access token.</param>
/// <param name="AccessTokenExpires">The UTC expiration date of the access token.</param>
/// <param name="RefreshToken">The issued refresh token.</param>
public record AuthResult(string AccessToken, DateTime AccessTokenExpires, string RefreshToken);
