using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

public class RoleConfigurator : GenericConfigurator<Role>
{
    public override Type[] DependsOn => [];
}