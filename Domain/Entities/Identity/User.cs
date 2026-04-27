// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public sealed class User : IdentityUser
{
    public User()
    {
    }

    public User(AuthRequest request)
    {
        UserName = request.UserName;
        Email = request.Email;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The date and time when the user was created. At UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
}