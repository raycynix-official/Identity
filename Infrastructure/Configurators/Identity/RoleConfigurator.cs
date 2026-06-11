using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

/// <summary>
/// Configures the role entity mapping.
/// </summary>
[DatabaseTable("roles")]
public class RoleConfigurator : GenericConfigurator<Role>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [];
}
