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
    /// Gets or sets the public page URL where the user chooses a new password.
    /// A root-relative value is resolved against the configured identity authority.
    /// </summary>
    public string PageUrl { get; set; } = "/reset-password";

    /// <summary>
    /// Validates the email confirmation configuration.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(TemplatePath))
        {
            throw new InvalidOperationException("Reset password template path is required");
        }

        if (string.IsNullOrWhiteSpace(PageUrl))
        {
            throw new InvalidOperationException("Reset password page URL is required");
        }

        if (Uri.TryCreate(PageUrl, UriKind.Absolute, out var absoluteUri))
        {
            if (!string.Equals(absoluteUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(absoluteUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Reset password page URL must use HTTP or HTTPS");
            }

            return;
        }

        if (!PageUrl.StartsWith("/", StringComparison.Ordinal) || PageUrl.StartsWith("//", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Reset password page URL must be absolute or root-relative");
        }
    }
}
