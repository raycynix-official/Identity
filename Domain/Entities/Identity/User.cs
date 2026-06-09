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
        CreatedAt = DateTime.UtcNow;
    }

    public User(RegisterRequest request)
    {
        UserName = request.UserName;
        Email = request.Email;
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; init; }

    public DateTime? LastLoginAt { get; set; }

    public List<UserRefreshToken>? RefreshTokens { get; set; } = null;
}