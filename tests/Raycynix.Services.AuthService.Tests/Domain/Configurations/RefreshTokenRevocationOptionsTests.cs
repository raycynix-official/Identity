using Raycynix.Services.AuthService.Domain.Configurations;

namespace Raycynix.Services.AuthService.Tests.Domain.Configurations;

public class RefreshTokenRevocationOptionsTests
{
    [Test]
    public void Constructor_UsesSecureDefaults()
    {
        var configuration = new RefreshTokenRevocationOptions();

        Assert.Multiple(() =>
        {
            Assert.That(configuration.RevokeExistingTokenOnLogin, Is.True);
            Assert.That(configuration.DetectReuseOnLogin, Is.True);
            Assert.That(configuration.DetectReuseOnRefresh, Is.True);
            Assert.That(configuration.RevokeActiveTokensOnReuse, Is.True);
            Assert.That(configuration.RevokeActiveTokensOnPasswordReset, Is.True);
            Assert.That(configuration.TrackLastUsedOnRefresh, Is.True);
        });
    }

    [Test]
    public void Validate_DoesNotThrow_ForDefaultConfiguration()
    {
        var configuration = new RefreshTokenRevocationOptions();

        Assert.DoesNotThrow(configuration.Validate);
    }
}
