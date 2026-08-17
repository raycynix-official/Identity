using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using Raycynix.Identity.Host.Infrastructure.Configurators;

namespace Raycynix.Identity.Host.Tests.Infrastructure.Configurators;

public class OpenIddictConfiguratorTests
{
    [Test]
    public void Configure_AddsOpenIddictEntitiesToModel()
    {
        var modelBuilder = new ModelBuilder();
        var configurator = new OpenIddictConfigurator();

        configurator.Configure(modelBuilder);

        Assert.Multiple(() =>
        {
            Assert.That(modelBuilder.Model.FindEntityType(typeof(OpenIddictEntityFrameworkCoreApplication)), Is.Not.Null);
            Assert.That(modelBuilder.Model.FindEntityType(typeof(OpenIddictEntityFrameworkCoreAuthorization)), Is.Not.Null);
            Assert.That(modelBuilder.Model.FindEntityType(typeof(OpenIddictEntityFrameworkCoreScope)), Is.Not.Null);
            Assert.That(modelBuilder.Model.FindEntityType(typeof(OpenIddictEntityFrameworkCoreToken)), Is.Not.Null);
        });
    }
}
