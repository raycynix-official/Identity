using Raycynix.Identity.Host.Domain.Configurations;

namespace Raycynix.Identity.Host.Tests.Domain.Configurations;

public class RateLimitOptionsTests
{
    [Test]
    public void Validate_DoesNotThrow_ForDefaultConfiguration()
    {
        var configuration = new RateLimitOptions();

        Assert.DoesNotThrow(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenPermitLimitIsNotPositive()
    {
        var configuration = new RateLimitOptions
        {
            PermitLimit = 0
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenWindowIsNotPositive()
    {
        var configuration = new RateLimitOptions
        {
            Window = TimeSpan.Zero
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }

    [Test]
    public void Validate_Throws_WhenQueueLimitIsNegative()
    {
        var configuration = new RateLimitOptions
        {
            QueueLimit = -1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => configuration.Validate());
    }
}
