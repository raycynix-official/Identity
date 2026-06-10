// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public sealed class User : IdentityUser<Guid>
{
    public User()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public User(RegisterRequest request)
    {
        UserName = request.UserName;
        Email = request.Email;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public List<UserRefreshToken> RefreshTokens { get; set; } = [];

    public List<UserRole> UserRoles { get; set; } = [];
    public List<UserClaim> UserClaims { get; set; } = [];
    public List<UserLogin> UserLogins { get; set; } = [];
    public List<UserToken> UserTokens { get; set; } = [];
}