using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Web.Controllers;

namespace Raycynix.Services.AuthService.Tests.Web.Controllers;

public class AuthControllerTests
{
    [Test]
    public void Controller_HasApiControllerAndBaseRouteAttributes()
    {
        var controllerType = typeof(AuthController);

        Assert.Multiple(() =>
        {
            Assert.That(controllerType.GetCustomAttribute<ApiControllerAttribute>(), Is.Not.Null);
            Assert.That(controllerType.GetCustomAttribute<RouteAttribute>()?.Template, Is.EqualTo(AuthRoutes.Base));
        });
    }

    [TestCase(nameof(AuthController.RegisterAsync), AuthRoutes.Registration, StatusCodes.Status200OK)]
    [TestCase(nameof(AuthController.LoginAsync), AuthRoutes.Login, StatusCodes.Status200OK)]
    [TestCase(nameof(AuthController.SendEmailConfirmationLinkAsync), AuthRoutes.EmailConfirmationSend,
        StatusCodes.Status204NoContent)]
    [TestCase(nameof(AuthController.SendPasswordResetLinkAsync), AuthRoutes.PasswordResetSend,
        StatusCodes.Status204NoContent)]
    [TestCase(nameof(AuthController.ResetPasswordAsync), AuthRoutes.PasswordReset, StatusCodes.Status204NoContent)]
    [TestCase(nameof(AuthController.RefreshTokenAsync), AuthRoutes.Refresh, StatusCodes.Status200OK)]
    public void SensitivePostEndpoint_HasRouteRateLimitAnd429Response(string methodName, string route,
        int successStatusCode)
    {
        var method = GetSingleMethod(methodName);

        Assert.Multiple(() =>
        {
            Assert.That(method.GetCustomAttribute<HttpPostAttribute>()?.Template, Is.EqualTo(route));
            Assert.That(method.GetCustomAttribute<EnableRateLimitingAttribute>(), Is.Not.Null);
            Assert.That(HasResponseStatus(method, successStatusCode), Is.True);
            Assert.That(HasResponseStatus(method, StatusCodes.Status429TooManyRequests), Is.True);
        });
    }

    [Test]
    public void ConfirmEmailPostEndpoint_HasExpectedRouteAndNoContentResponse()
    {
        var method = GetMethod(nameof(AuthController.ConfirmEmailAsync), typeof(ConfirmEmailRequest),
            typeof(CancellationToken));

        Assert.Multiple(() =>
        {
            Assert.That(method.GetCustomAttribute<HttpPostAttribute>()?.Template,
                Is.EqualTo(AuthRoutes.EmailConfirmationConfirm));
            Assert.That(HasResponseStatus(method, StatusCodes.Status204NoContent), Is.True);
        });
    }

    [Test]
    public void ConfirmEmailGetEndpoint_HasExpectedRouteAndNoContentResponse()
    {
        var method = GetMethod(nameof(AuthController.ConfirmEmailAsync), typeof(string), typeof(string),
            typeof(CancellationToken));

        Assert.Multiple(() =>
        {
            Assert.That(method.GetCustomAttribute<HttpGetAttribute>()?.Template,
                Is.EqualTo(AuthRoutes.EmailConfirmationConfirm));
            Assert.That(HasResponseStatus(method, StatusCodes.Status204NoContent), Is.True);
        });
    }

    [Test]
    public void LogoutEndpoint_HasExpectedRouteAndNoContentResponse()
    {
        var method = GetSingleMethod(nameof(AuthController.LogoutAsync));

        Assert.Multiple(() =>
        {
            Assert.That(method.GetCustomAttribute<HttpPostAttribute>()?.Template, Is.EqualTo(AuthRoutes.Logout));
            Assert.That(HasResponseStatus(method, StatusCodes.Status204NoContent), Is.True);
        });
    }

    private static MethodInfo GetSingleMethod(string methodName)
    {
        return typeof(AuthController).GetMethods()
            .Single(method => method.Name == methodName);
    }

    private static MethodInfo GetMethod(string methodName, params Type[] parameterTypes)
    {
        return typeof(AuthController).GetMethod(methodName, parameterTypes)
               ?? throw new InvalidOperationException($"Method {methodName} was not found.");
    }

    private static bool HasResponseStatus(MethodInfo method, int statusCode)
    {
        return method.GetCustomAttributes<ProducesResponseTypeAttribute>()
            .Any(attribute => attribute.StatusCode == statusCode);
    }
}
