using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using Raycynix.Extensions.Database.Abstractions.Configurators;

namespace Raycynix.Identity.Host.Infrastructure.Configurators;

/// <summary>
/// Adds the OpenIddict persistence model to the shared Identity database context.
/// </summary>
public sealed class OpenIddictConfigurator : IConfigurator
{
    /// <inheritdoc />
    public Type Type => typeof(OpenIddictEntityFrameworkCoreApplication);

    /// <inheritdoc />
    public Type[] DependsOn => [];

    /// <inheritdoc />
    public string ModelCacheKey => nameof(OpenIddictConfigurator);

    /// <inheritdoc />
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.UseOpenIddict();
    }

    /// <inheritdoc />
    public void Seed(ModelBuilder modelBuilder)
    {
    }
}
