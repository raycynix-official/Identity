using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using Raycynix.Identity.Host.Web.Extensions;

namespace Raycynix.Identity.Host.Tests.Web.Extensions;

public class OpenIddictDependencyInjectionTests
{
    [Test]
    public void AddRaycynixOpenIddict_WhenDisabled_DoesNotRegisterOpenIddictManagers()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(enabled: false);

        services.AddRaycynixOpenIddict(configuration, CreateEnvironment());

        Assert.That(
            services.Any(descriptor => descriptor.ServiceType == typeof(IOpenIddictApplicationManager)),
            Is.False);
    }

    [Test]
    public void AddRaycynixOpenIddict_WhenEnabled_RegistersOpenIddictManagers()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(enabled: true);

        services.AddRaycynixOpenIddict(configuration, CreateEnvironment());

        Assert.That(
            services.Any(descriptor => descriptor.ServiceType == typeof(IOpenIddictApplicationManager)),
            Is.True);
    }

    private static IConfiguration CreateConfiguration(bool enabled)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenIddictOptions:Enabled"] = enabled.ToString(),
                ["OpenIddictOptions:Issuer"] = "https://id.example.com/",
                ["OpenIddictOptions:UseDevelopmentCertificates"] = "true"
            })
            .Build();
    }

    private static IHostEnvironment CreateEnvironment()
    {
        return new TestHostEnvironment
        {
            ApplicationName = "Raycynix.Identity.Host.Tests",
            EnvironmentName = Environments.Development,
            ContentRootPath = AppContext.BaseDirectory,
            ContentRootFileProvider = new NullFileProvider()
        };
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public required string EnvironmentName { get; set; }

        public required string ApplicationName { get; set; }

        public required string ContentRootPath { get; set; }

        public required IFileProvider ContentRootFileProvider { get; set; }
    }
}
