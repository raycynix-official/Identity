using Raycynix.Extensions.Security.Abstractions.Interfaces;
using Raycynix.Identity.Host.Application.Extensions;

namespace Raycynix.Identity.Host.Tests.Application.Extensions;

public class SecretResolverExtensionsTests
{
    [Test]
    public async Task GetRefreshTokenHashSecretAsync_WhenDedicatedSecretExists_ReturnsDedicatedSecret()
    {
        var resolver = new FakeSecretResolver(new Dictionary<string, string?>
        {
            ["SecurityConfiguration:RefreshTokenHashSecret"] = "refresh-token-secret",
            ["SecurityConfiguration:Jwt:Secret"] = "jwt-secret"
        });

        var secret = await resolver.GetRefreshTokenHashSecretAsync();

        Assert.That(secret, Is.EqualTo("refresh-token-secret"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task GetRefreshTokenHashSecretAsync_WhenDedicatedSecretMissing_ReturnsJwtSecret(string? dedicatedSecret)
    {
        var resolver = new FakeSecretResolver(new Dictionary<string, string?>
        {
            ["SecurityConfiguration:RefreshTokenHashSecret"] = dedicatedSecret,
            ["SecurityConfiguration:Jwt:Secret"] = "jwt-secret"
        });

        var secret = await resolver.GetRefreshTokenHashSecretAsync();

        Assert.That(secret, Is.EqualTo("jwt-secret"));
    }

    [Test]
    public void GetRefreshTokenHashSecretAsync_WhenNoSecretExists_Throws()
    {
        var resolver = new FakeSecretResolver(new Dictionary<string, string?>());

        Assert.ThrowsAsync<InvalidOperationException>(async () => await resolver.GetRefreshTokenHashSecretAsync());
    }

    private sealed class FakeSecretResolver(IReadOnlyDictionary<string, string?> secrets) : ISecretResolver
    {
        public ValueTask<string?> GetSecretAsync(string key, CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult(secrets.GetValueOrDefault(key));
        }
    }
}
