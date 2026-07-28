namespace Raycynix.Identity.Host.Domain.Configurations;

/// <summary>
/// Contains reset password delivery settings
/// </summary>
public class ResetPasswordOptions
{
    /// <summary>
    /// Gets or sets the relative or absolute path to the email confirmation HTML template.
    /// </summary>
    public string TemplatePath { get; set; } = "Templates/Emails/PasswordReset.html";

    /// <summary>
    /// Validates the email confirmation configuration.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(TemplatePath))
        {
            throw new InvalidOperationException("Reset password template path is required");
        }
    }
}