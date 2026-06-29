// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class User : IdentityUser<Guid>
{
    /// <summary>
    /// Initializes a new empty user instance.
    /// </summary>
    public User()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Initializes a new user instance from a registration request.
    /// </summary>
    /// <param name="request">The registration data used to populate the user.</param>
    public User(RegisterRequest request)
    {
        UserName = request.UserName.Trim();
        Email = request.Email.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the UTC date and time when the user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the UTC date and time when the user last logged in.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the refresh tokens issued to the user.
    /// </summary>
    public List<UserRefreshToken> RefreshTokens { get; set; } = [];

    /// <summary>
    /// Gets or sets the roles assigned to the user.
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the claims assigned to the user.
    /// </summary>
    public List<UserClaim> UserClaims { get; set; } = [];

    /// <summary>
    /// Gets or sets the external logins associated with the user.
    /// </summary>
    public List<UserLogin> UserLogins { get; set; } = [];

    /// <summary>
    /// Gets or sets the Identity tokens associated with the user.
    /// </summary>
    public List<UserToken> UserTokens { get; set; } = [];
}
