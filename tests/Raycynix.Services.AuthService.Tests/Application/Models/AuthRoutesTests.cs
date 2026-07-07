using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Tests.Application.Models;

public class AuthRoutesTests
{
    [Test]
    public void RouteConstants_MatchPublicApiContract()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AuthRoutes.Base, Is.EqualTo("api/v1/auth"));
            Assert.That(AuthRoutes.Registration, Is.EqualTo("registration"));
            Assert.That(AuthRoutes.Login, Is.EqualTo("login"));
            Assert.That(AuthRoutes.EmailConfirmationSend, Is.EqualTo("email-confirmation/send"));
            Assert.That(AuthRoutes.EmailConfirmationConfirm, Is.EqualTo("email-confirmation/confirm"));
            Assert.That(AuthRoutes.PasswordResetSend, Is.EqualTo("password-reset/send"));
            Assert.That(AuthRoutes.PasswordReset, Is.EqualTo("password-reset/reset"));
            Assert.That(AuthRoutes.Logout, Is.EqualTo("logout"));
            Assert.That(AuthRoutes.Refresh, Is.EqualTo("refresh"));
        });
    }

    [Test]
    public void LinkPaths_CombineBaseRouteAndEndpointRoute()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                AuthRoutes.EmailConfirmationConfirmPath,
                Is.EqualTo("api/v1/auth/email-confirmation/confirm"));
            Assert.That(
                AuthRoutes.PasswordResetPath,
                Is.EqualTo("api/v1/auth/password-reset/reset"));
        });
    }
}
