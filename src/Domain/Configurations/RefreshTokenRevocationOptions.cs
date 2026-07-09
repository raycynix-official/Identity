// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Configurations;

/// <summary>
/// Contains refresh-token revocation policy settings.
/// </summary>
public class RefreshTokenRevocationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether an existing active refresh-token cookie should be revoked on login.
    /// </summary>
    public bool RevokeExistingTokenOnLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether revoked refresh-token reuse should be detected during login.
    /// </summary>
    public bool DetectReuseOnLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether revoked refresh-token reuse should be detected during refresh.
    /// </summary>
    public bool DetectReuseOnRefresh { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether active refresh tokens should be revoked after reuse is detected.
    /// </summary>
    public bool RevokeActiveTokensOnReuse { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether active refresh tokens should be revoked after a password reset.
    /// </summary>
    public bool RevokeActiveTokensOnPasswordReset { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether successful refresh operations should update token last-used timestamps.
    /// </summary>
    public bool TrackLastUsedOnRefresh { get; set; } = true;

    /// <summary>
    /// Validates the refresh-token revocation configuration.
    /// </summary>
    public void Validate()
    {
    }
}
