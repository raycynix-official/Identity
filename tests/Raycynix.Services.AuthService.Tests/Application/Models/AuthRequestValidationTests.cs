using System.ComponentModel.DataAnnotations;
using Raycynix.Services.AuthService.Application.Models;

namespace Raycynix.Services.AuthService.Tests.Application.Models;

public class AuthRequestValidationTests
{
    [Test]
    public void RegisterRequest_IsValid_WhenRequiredFieldsAreProvided()
    {
        var request = new RegisterRequest("user", "user@example.com", "Password1");

        var results = Validate(request);

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void RegisterRequest_IsInvalid_WhenEmailIsInvalid()
    {
        var request = new RegisterRequest("user", "not-an-email", "Password1");

        var results = Validate(request);

        Assert.That(results.SelectMany(result => result.MemberNames), Does.Contain(nameof(RegisterRequest.Email)));
    }

    [Test]
    public void RegisterRequest_IsInvalid_WhenUserNameIsWhitespace()
    {
        var request = new RegisterRequest("   ", "user@example.com", "Password1");

        var results = Validate(request);

        Assert.That(results.SelectMany(result => result.MemberNames), Does.Contain(nameof(RegisterRequest.UserName)));
    }

    [Test]
    public void LoginRequest_IsInvalid_WhenLoginIsWhitespace()
    {
        var request = new LoginRequest("   ", "Password1");

        var results = Validate(request);

        Assert.That(results.SelectMany(result => result.MemberNames), Does.Contain(nameof(LoginRequest.Login)));
    }

    [Test]
    public void ConfirmEmailRequest_IsInvalid_WhenTokenIsWhitespace()
    {
        var request = new ConfirmEmailRequest("user@example.com", "   ");

        var results = Validate(request);

        Assert.That(results.SelectMany(result => result.MemberNames), Does.Contain(nameof(ConfirmEmailRequest.Token)));
    }

    [Test]
    public void EmailConfirmationLinkRequest_IsInvalid_WhenEmailIsInvalid()
    {
        var request = new EmailConfirmationLinkRequest("not-an-email");

        var results = Validate(request);

        Assert.That(
            results.SelectMany(result => result.MemberNames),
            Does.Contain(nameof(EmailConfirmationLinkRequest.Email)));
    }

    [Test]
    public void PasswordResetLinkRequest_IsInvalid_WhenEmailIsInvalid()
    {
        var request = new PasswordResetLinkRequest("not-an-email");

        var results = Validate(request);

        Assert.That(
            results.SelectMany(result => result.MemberNames),
            Does.Contain(nameof(PasswordResetLinkRequest.Email)));
    }

    [Test]
    public void ResetPasswordRequest_IsInvalid_WhenNewPasswordIsWhitespace()
    {
        var request = new ResetPasswordRequest("user@example.com", "token", "   ");

        var results = Validate(request);

        Assert.That(results.SelectMany(result => result.MemberNames), Does.Contain(nameof(ResetPasswordRequest.NewPassword)));
    }

    private static IReadOnlyCollection<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            instance,
            new ValidationContext(instance),
            results,
            validateAllProperties: true);

        return results;
    }
}
