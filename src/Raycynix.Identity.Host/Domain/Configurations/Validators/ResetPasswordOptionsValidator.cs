using Raycynix.Extensions.Configuration.Abstractions.Interfaces;
using Raycynix.Extensions.Configuration.Abstractions.Models;

namespace Raycynix.Identity.Host.Domain.Configurations.Validators;

/// <summary>
/// Validates reset password configuration values. 
/// </summary>
public sealed class ResetPasswordOptionsValidator : IConfigurationValidator<ResetPasswordOptions>
{
    /// <inheritdoc />
    public ConfigurationValidationResult Validate(ResetPasswordOptions options)
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