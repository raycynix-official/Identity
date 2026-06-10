using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public sealed class UserClaim : IdentityUserClaim<Guid>
{
    public User User { get; set; } = null!;
}