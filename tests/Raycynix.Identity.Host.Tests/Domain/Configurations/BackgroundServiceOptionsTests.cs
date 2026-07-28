using Raycynix.Identity.Host.Domain.Configurations.BackgroundServices;

namespace Raycynix.Identity.Host.Tests.Domain.Configurations;

public class BackgroundServiceOptionsTests
{
    [Test]
    public void Validate_DefaultConfiguration_DoesNotThrow()
    {
        var configuration = new BackgroundServiceOptions();

        Assert.DoesNotThrow(configuration.Validate);
    }

    [Test]
    public void Validate_WhenCleanupDisabledAllowsZeroInterval_DoesNotThrow()
    {
        var configuration = new BackgroundServiceOptions
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
        var configuration = new BackgroundServiceOptions
        {
            RefreshTokensCleanupEnabled = true,
            RefreshTokensCleanupInterval = TimeSpan.FromSeconds(seconds)
        };

        Assert.Throws<ArgumentOutOfRangeException>(configuration.Validate);
    }
}
