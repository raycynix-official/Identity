// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Configurations;

/// <summary>
/// Contains rate-limit settings for sensitive authentication endpoints.
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Gets or sets the maximum number of permitted requests in a fixed window.
    /// </summary>
    public int PermitLimit { get; set; } = 10;

    /// <summary>
    /// Gets or sets the fixed-window duration.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the maximum number of requests queued after the permit limit is reached.
    /// </summary>
    public int QueueLimit { get; set; }

    /// <summary>
    /// Validates the rate-limit configuration.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when permit limit, window, or queue limit values are outside supported ranges.
    /// </exception>
    public void Validate()
    {
        if (PermitLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(PermitLimit), "Rate-limit permit limit must be greater than zero.");

        if (Window <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(Window), "Rate-limit window must be greater than zero.");

        if (QueueLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(QueueLimit), "Rate-limit queue limit cannot be negative.");
    }
}
