// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Configurations;

/// <summary>
/// Contains email confirmation delivery settings.
/// </summary>
public class EmailConfirmationConfiguration
{
    /// <summary>
    /// Gets or sets the public email confirmation endpoint URL without query parameters.
    /// </summary>
    public string ConfirmationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Validates the email confirmation configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the confirmation URL is missing or invalid.
    /// </exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConfirmationUrl))
        {
            throw new InvalidOperationException("Email confirmation URL is required");
        }

        if (!Uri.TryCreate(ConfirmationUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("Email confirmation URL must be an absolute URL");
        }
    }
}
