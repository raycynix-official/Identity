using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public sealed class UserRole : IdentityUserRole<Guid>
{
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}