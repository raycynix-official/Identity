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
public class EmailConfirmationOptions
{
    /// <summary>
    /// Gets or sets the relative or absolute path to the email confirmation HTML template.
    /// </summary>
    public string TemplatePath { get; set; } = "Templates/Emails/EmailConfirmation.html";

    /// <summary>
    /// Validates the email confirmation configuration.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(TemplatePath))
        {
            throw new InvalidOperationException("Email confirmation template path is required");
        }
    }
}
