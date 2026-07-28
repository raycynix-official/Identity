using Raycynix.Identity.Host.Application.Extensions;

namespace Raycynix.Identity.Host.Tests.Application.Extensions;

public class SecurityExtensionsTests
{
    [Test]
    public void HashRefreshToken_ReturnsStableHmacHash_ForSameTokenAndSecret()
    {
        const string refreshToken = "raw-refresh-token";
        const string secret = "0123456789abcdef0123456789abcdef";

        var firstHash = SecurityExtensions.HashRefreshToken(refreshToken, secret);
        var secondHash = SecurityExtensions.HashRefreshToken(refreshToken, secret);

        Assert.That(secondHash, Is.EqualTo(firstHash));
    }

    [Test]
    public void HashRefreshToken_ReturnsDifferentHash_WhenSecretChanges()
    {
        const string refreshToken = "raw-refresh-token";

        var firstHash = SecurityExtensions.HashRefreshToken(
            refreshToken,
            "0123456789abcdef0123456789abcdef");
        var secondHash = SecurityExtensions.HashRefreshToken(
            refreshToken,
            "abcdef0123456789abcdef0123456789");

        Assert.That(secondHash, Is.Not.EqualTo(firstHash));
    }

    [Test]
    public void HashRefreshToken_DoesNotMatchLegacySha256Hash()
    {
        const string refreshToken = "raw-refresh-token";
        const string secret = "0123456789abcdef0123456789abcdef";

        var hmacHash = SecurityExtensions.HashRefreshToken(refreshToken, secret);
        var legacyHash = SecurityExtensions.HashRefreshTokenLegacy(refreshToken);

        Assert.That(hmacHash, Is.Not.EqualTo(legacyHash));
    }

    [Test]
    public void GenerateRefreshToken_ReturnsBase64EncodedRandomToken()
    {
        var firstToken = SecurityExtensions.GenerateRefreshToken();
        var secondToken = SecurityExtensions.GenerateRefreshToken();

        Assert.Multiple(() =>
        {
            Assert.That(Convert.FromBase64String(firstToken), Has.Length.EqualTo(64));
            Assert.That(Convert.FromBase64String(secondToken), Has.Length.EqualTo(64));
            Assert.That(secondToken, Is.Not.EqualTo(firstToken));
        });
    }
}
