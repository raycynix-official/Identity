// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

/// <summary>
/// Represents a persisted refresh token issued to a user.
/// </summary>
public sealed class UserRefreshToken
{
    /// <summary>
    /// Gets the refresh-token identifier.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the identifier of the user that owns the refresh token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the refresh token.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hash of the raw refresh token.
    /// </summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// Gets the UTC date and time when the refresh token was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the UTC date and time when the refresh token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time when the refresh token was revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the hash of the refresh token that replaced this token.
    /// </summary>
    public string? ReplacedByTokenHash { get; set; }

    /// <summary>
    /// Gets or sets the reason why the refresh token was revoked.
    /// </summary>
    public UserRefreshTokenRevocationReason? RevocationReason { get; set; }

    /// <summary>
    /// Gets a value indicating whether the refresh token can still be used.
    /// </summary>
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
