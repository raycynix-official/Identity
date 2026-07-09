// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Raycynix.Extensions.Configuration.Abstractions.Interfaces;
using Raycynix.Extensions.Configuration.Abstractions.Models;

namespace Raycynix.Services.AuthService.Domain.Configurations.Validators;

/// <summary>
/// Validates refresh-token revocation configuration values.
/// </summary>
public sealed class RefreshTokenRevocationOptionsValidator
    : IConfigurationValidator<RefreshTokenRevocationOptions>
{
    /// <inheritdoc />
    public ConfigurationValidationResult Validate(RefreshTokenRevocationOptions options)
    {
        try
        {
            options.Validate();
            return ConfigurationValidationResult.Success();
        }
        catch (Exception exception)
        {
            return ConfigurationValidationResult.Failure(exception.Message);
        }
    }
}
