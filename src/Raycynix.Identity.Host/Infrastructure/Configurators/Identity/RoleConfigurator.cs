using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Infrastructure.Configurators.Identity;

/// <summary>
/// Configures the role entity mapping.
/// </summary>
[DatabaseTable("roles")]
public class RoleConfigurator : GenericConfigurator<Role>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [];
}
