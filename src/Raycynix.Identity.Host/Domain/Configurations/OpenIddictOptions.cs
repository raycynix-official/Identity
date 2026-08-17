namespace Raycynix.Identity.Host.Domain.Configurations;

/// <summary>
/// Contains configuration for the OAuth 2.0 and OpenID Connect server.
/// </summary>
public sealed class OpenIddictOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the OpenIddict server is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the absolute issuer URI advertised in discovery metadata and tokens.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether OpenIddict development certificates should be used.
    /// This option is rejected outside the Development environment.
    /// </summary>
    public bool UseDevelopmentCertificates { get; set; }

    /// <summary>
    /// Gets or sets the path to the production signing certificate in PKCS#12 format.
    /// </summary>
    public string? SigningCertificatePath { get; set; }

    /// <summary>
    /// Gets or sets the password used to load the production signing certificate.
    /// </summary>
    public string? SigningCertificatePassword { get; set; }

    /// <summary>
    /// Gets or sets the path to the production encryption certificate in PKCS#12 format.
    /// </summary>
    public string? EncryptionCertificatePath { get; set; }

    /// <summary>
    /// Gets or sets the password used to load the production encryption certificate.
    /// </summary>
    public string? EncryptionCertificatePassword { get; set; }

    /// <summary>
    /// Validates the configuration for the current hosting environment.
    /// </summary>
    /// <param name="isDevelopment">Whether the application is running in Development.</param>
    public void Validate(bool isDevelopment)
    {
        if (!Enabled)
        {
            return;
        }

        if (!Uri.TryCreate(Issuer, UriKind.Absolute, out var issuer) ||
            issuer.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(issuer.Query) ||
            !string.IsNullOrEmpty(issuer.Fragment))
        {
            throw new InvalidOperationException(
                "OpenIddict issuer must be an absolute HTTPS URI without query or fragment components");
        }

        if (UseDevelopmentCertificates)
        {
            if (!isDevelopment)
            {
                throw new InvalidOperationException(
                    "OpenIddict development certificates cannot be used outside the Development environment");
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(SigningCertificatePath))
        {
            throw new InvalidOperationException("OpenIddict signing certificate path is required");
        }

        if (string.IsNullOrWhiteSpace(EncryptionCertificatePath))
        {
            throw new InvalidOperationException("OpenIddict encryption certificate path is required");
        }
    }
}
