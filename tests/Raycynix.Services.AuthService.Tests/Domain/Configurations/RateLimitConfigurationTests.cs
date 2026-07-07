using Raycynix.Services.AuthService.Domain.Configurations;

namespace Raycynix.Services.AuthService.Tests.Domain.Configurations;

public class RateLimitConfigurationTests
{
    [Test]
    public void Validate_DoesNotThrow_ForDefaultConfiguration()
    {
        var configuration = new RateLimitConfiguration();

        Assert.DoesNotThrow(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenPermitLimitIsNotPositive()
    {
        var configuration = new RateLimitConfiguration
        {
            PermitLimit = 0
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenWindowIsNotPositive()
    {
        var configuration = new RateLimitConfiguration
        {
            Window = TimeSpan.Zero
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenQueueLimitIsNegative()
    {
        var configuration = new RateLimitConfiguration
        {
            QueueLimit = -1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }
}
