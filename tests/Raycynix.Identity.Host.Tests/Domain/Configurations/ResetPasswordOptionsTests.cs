using Raycynix.Identity.Host.Domain.Configurations;

namespace Raycynix.Identity.Host.Tests.Domain.Configurations;

public class ResetPasswordOptionsTests
{
    [Test]
    public void Validate_DefaultConfiguration_DoesNotThrow()
    {
        var configuration = new ResetPasswordOptions();

        Assert.DoesNotThrow(configuration.Validate);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Validate_WhenTemplatePathIsMissing_Throws(string templatePath)
    {
        var configuration = new ResetPasswordOptions
        {
            TemplatePath = templatePath
        };

        Assert.Throws<InvalidOperationException>(configuration.Validate);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("reset-password")]
    [TestCase("//attacker.example/reset-password")]
    [TestCase("javascript:alert(1)")]
    public void Validate_WhenPageUrlIsInvalid_Throws(string pageUrl)
    {
        var configuration = new ResetPasswordOptions
        {
            PageUrl = pageUrl
        };

        Assert.Throws<InvalidOperationException>(configuration.Validate);
    }

    [TestCase("/reset-password")]
    [TestCase("https://accounts.example.com/reset-password")]
    [TestCase("http://localhost:3000/reset-password")]
    public void Validate_WhenPageUrlIsValid_DoesNotThrow(string pageUrl)
    {
        var configuration = new ResetPasswordOptions
        {
            PageUrl = pageUrl
        };

        Assert.DoesNotThrow(configuration.Validate);
    }
}
