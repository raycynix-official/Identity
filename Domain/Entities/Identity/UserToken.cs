using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public class UserToken : IdentityUserToken<Guid>
{
    public User User { get; set; } = null!;
}