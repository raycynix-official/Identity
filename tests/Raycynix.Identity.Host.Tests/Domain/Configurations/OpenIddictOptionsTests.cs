using Raycynix.Identity.Host.Domain.Configurations;

namespace Raycynix.Identity.Host.Tests.Domain.Configurations;

public class OpenIddictOptionsTests
{
    [Test]
    public void Validate_WhenDisabled_AllowsMissingServerSettings()
    {
        var options = new OpenIddictOptions();

        Assert.DoesNotThrow(() => options.Validate(isDevelopment: false));
    }

    [Test]
    public void Validate_WhenDevelopmentCertificatesAreUsedInDevelopment_DoesNotThrow()
    {
        var options = CreateEnabledOptions();
        options.UseDevelopmentCertificates = true;

        Assert.DoesNotThrow(() => options.Validate(isDevelopment: true));
    }

    [Test]
    public void Validate_WhenDevelopmentCertificatesAreUsedInProduction_Throws()
    {
        var options = CreateEnabledOptions();
        options.UseDevelopmentCertificates = true;

        Assert.Throws<InvalidOperationException>(() => options.Validate(isDevelopment: false));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("/identity")]
    [TestCase("http://id.example.com")]
    [TestCase("https://id.example.com?tenant=one")]
    [TestCase("https://id.example.com/#fragment")]
    public void Validate_WhenIssuerIsInvalid_Throws(string? issuer)
    {
        var options = CreateEnabledOptions();
        options.Issuer = issuer;
        options.UseDevelopmentCertificates = true;

        Assert.Throws<InvalidOperationException>(() => options.Validate(isDevelopment: true));
    }

    [Test]
    public void Validate_WhenProductionCertificatePathsArePresent_DoesNotThrow()
    {
        var options = CreateEnabledOptions();

        Assert.DoesNotThrow(() => options.Validate(isDevelopment: false));
    }

    [TestCase(null, "encryption.pfx")]
    [TestCase("", "encryption.pfx")]
    [TestCase("signing.pfx", null)]
    [TestCase("signing.pfx", "")]
    public void Validate_WhenProductionCertificatePathIsMissing_Throws(
        string? signingCertificatePath,
        string? encryptionCertificatePath)
    {
        var options = CreateEnabledOptions();
        options.SigningCertificatePath = signingCertificatePath;
        options.EncryptionCertificatePath = encryptionCertificatePath;

        Assert.Throws<InvalidOperationException>(() => options.Validate(isDevelopment: false));
    }

    private static OpenIddictOptions CreateEnabledOptions()
    {
        return new OpenIddictOptions
        {
            Enabled = true,
            Issuer = "https://id.example.com/",
            SigningCertificatePath = "signing.pfx",
            EncryptionCertificatePath = "encryption.pfx"
        };
    }
}
