using Raycynix.Services.AuthService.Domain.Configurations;

namespace Raycynix.Services.AuthService.Tests.Domain.Configurations;

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
}
