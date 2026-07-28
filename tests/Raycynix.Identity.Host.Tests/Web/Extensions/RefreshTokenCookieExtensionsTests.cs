using Microsoft.AspNetCore.Http;
using Raycynix.Identity.Host.Web.Extensions;

namespace Raycynix.Identity.Host.Tests.Web.Extensions;

public class RefreshTokenCookieExtensionsTests
{
    [Test]
    public void AppendRefreshToken_WritesSecureRefreshTokenCookie()
    {
        var context = new DefaultHttpContext();

        context.Response.Cookies.AppendRefreshToken("refresh-token", TimeSpan.FromMinutes(5));

        var setCookie = context.Response.Headers.SetCookie.Single()!;
        Assert.Multiple(() =>
        {
            Assert.That(setCookie, Does.StartWith("refresh_token=refresh-token;"));
            Assert.That(setCookie, Does.Contain("httponly").IgnoreCase);
            Assert.That(setCookie, Does.Contain("secure").IgnoreCase);
            Assert.That(setCookie, Does.Contain("samesite=strict").IgnoreCase);
            Assert.That(setCookie, Does.Contain("path=/api/v1/auth").IgnoreCase);
            Assert.That(setCookie, Does.Contain("max-age=300").IgnoreCase);
        });
    }

    [Test]
    public void DeleteRefreshToken_WritesExpiredRefreshTokenCookie()
    {
        var context = new DefaultHttpContext();

        context.Response.Cookies.DeleteRefreshToken();

        var setCookie = context.Response.Headers.SetCookie.Single()!;
        Assert.Multiple(() =>
        {
            Assert.That(setCookie, Does.StartWith("refresh_token=;"));
            Assert.That(setCookie, Does.Contain("expires=Thu, 01 Jan 1970 00:00:00 GMT").IgnoreCase);
            Assert.That(setCookie, Does.Contain("path=/api/v1/auth").IgnoreCase);
        });
    }

    [Test]
    public void GetRefreshToken_ReturnsRefreshTokenCookieValue()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "other=value; refresh_token=raw-token";

        var refreshToken = context.Request.Cookies.GetRefreshToken();

        Assert.That(refreshToken, Is.EqualTo("raw-token"));
    }
}
