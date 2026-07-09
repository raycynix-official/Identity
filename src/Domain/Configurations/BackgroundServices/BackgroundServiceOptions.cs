// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Configurations.BackgroundServices;

/// <summary>
/// Contains settings for optional background services.
/// </summary>
public class BackgroundServiceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether expired refresh tokens should be cleaned up in the background.
    /// </summary>
    public bool RefreshTokensCleanupEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the interval between expired refresh-token cleanup runs.
    /// </summary>
    public TimeSpan RefreshTokensCleanupInterval { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Validates the background service configuration.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when refresh-token cleanup is enabled, and the cleanup interval is not greater than zero.
    /// </exception>
    public void Validate()
    {
        if (RefreshTokensCleanupEnabled && RefreshTokensCleanupInterval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(RefreshTokensCleanupInterval), "Refresh tokens interval cannot be negative.");
    }
}
