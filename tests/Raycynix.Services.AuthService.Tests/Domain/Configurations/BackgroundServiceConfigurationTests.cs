using Raycynix.Services.AuthService.Domain.Configurations.BackgroundServices;

namespace Raycynix.Services.AuthService.Tests.Domain.Configurations;

public class BackgroundServiceConfigurationTests
{
    [Test]
    public void Validate_DefaultConfiguration_DoesNotThrow()
    {
        var configuration = new BackgroundServiceConfiguration();

        Assert.DoesNotThrow(configuration.Validate);
    }

    [Test]
    public void Validate_WhenCleanupDisabledAllowsZeroInterval_DoesNotThrow()
    {
        var configuration = new BackgroundServiceConfiguration
        {
            RefreshTokensCleanupEnabled = false,
            RefreshTokensCleanupInterval = TimeSpan.Zero
        };

        Assert.DoesNotThrow(configuration.Validate);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Validate_WhenCleanupEnabledAndIntervalIsNotPositive_Throws(int seconds)
    {
        var configuration = new BackgroundServiceConfiguration
        {
            RefreshTokensCleanupEnabled = true,
            RefreshTokensCleanupInterval = TimeSpan.FromSeconds(seconds)
        };

        Assert.Throws<ArgumentOutOfRangeException>(configuration.Validate);
    }
}
