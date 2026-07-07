using Raycynix.Services.AuthService.Domain.Configurations;

namespace Raycynix.Services.AuthService.Tests.Domain.Configurations;

public class ResetPasswordConfigurationTests
{
    [Test]
    public void Validate_DefaultConfiguration_DoesNotThrow()
    {
        var configuration = new ResetPasswordConfiguration();

        Assert.DoesNotThrow(configuration.Validate);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Validate_WhenTemplatePathIsMissing_Throws(string templatePath)
    {
        var configuration = new ResetPasswordConfiguration
        {
            TemplatePath = templatePath
        };

        Assert.Throws<InvalidOperationException>(configuration.Validate);
    }
}
