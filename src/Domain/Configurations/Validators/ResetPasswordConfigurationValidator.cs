using Raycynix.Extensions.Configuration.Abstractions.Interfaces;
using Raycynix.Extensions.Configuration.Abstractions.Models;

namespace Raycynix.Services.AuthService.Domain.Configurations.Validators;

/// <summary>
/// Validates reset password configuration values. 
/// </summary>
public sealed class ResetPasswordConfigurationValidator : IConfigurationValidator<ResetPasswordConfiguration>
{
    /// <inheritdoc />
    public ConfigurationValidationResult Validate(ResetPasswordConfiguration options)
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