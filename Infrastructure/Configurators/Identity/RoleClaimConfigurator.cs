using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

public class RoleClaimConfigurator : GenericConfigurator<RoleClaim>
{
    public override Type[] DependsOn => [];
}