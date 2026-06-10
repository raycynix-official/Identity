using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public class RoleClaim : IdentityRoleClaim<Guid>
{
    public Role Role { get; set; } = null!;
}