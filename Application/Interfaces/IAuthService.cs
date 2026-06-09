// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Security.Claims;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Domain.Entities;

namespace Raycynix.Services.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(ClaimsPrincipal claims, string? refreshToken, CancellationToken cancellationToken = default);
}