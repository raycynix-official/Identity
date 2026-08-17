using Raycynix.Extensions.Security.Options;
using Raycynix.Identity.Host.Application.Extensions;
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Tests.Domain.Entities.Identity;

public class UserRefreshTokenTests
{
    [Test]
    public void GenerateUserRefreshToken_StoresProvidedTokenHash()
    {
        var user = new User
        {
            Id = Guid.NewGuid()
        };
        var jwtConfiguration = new JwtOptions()
        {
            RefreshTokenLifetime = TimeSpan.FromDays(14)
        };
        const string tokenHash = "precomputed-token-hash";

        var refreshToken = user.GenerateUserRefreshToken(tokenHash, jwtConfiguration);

        Assert.Multiple(() =>
        {
            Assert.That(refreshToken.UserId, Is.EqualTo(user.Id));
            Assert.That(refreshToken.TokenHash, Is.EqualTo(tokenHash));
            Assert.That(refreshToken.ExpiresAt, Is.GreaterThan(DateTimeOffset.UtcNow.AddDays(13)));
            Assert.That(refreshToken.IsActive, Is.True);
        });
    }

    [Test]
    public void IsActive_ReturnsFalse_WhenTokenIsRevoked()
    {
        var refreshToken = new UserRefreshToken
        {
            TokenHash = "hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            RevokedAt = DateTimeOffset.UtcNow,
            RevocationReason = UserRefreshTokenRevocationReason.Logout
        };

        Assert.That(refreshToken.IsActive, Is.False);
    }

    [Test]
    public void IsActive_ReturnsFalse_WhenTokenIsExpired()
    {
        var refreshToken = new UserRefreshToken
        {
            TokenHash = "hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(-1)
        };

        Assert.That(refreshToken.IsActive, Is.False);
    }
}
