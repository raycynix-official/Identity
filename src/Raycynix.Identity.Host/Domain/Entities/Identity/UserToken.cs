using Microsoft.AspNetCore.Identity;

namespace Raycynix.Identity.Host.Domain.Entities.Identity;

/// <summary>
/// Represents an Identity token associated with a user.
/// </summary>
public class UserToken : IdentityUserToken<Guid>
{
    /// <summary>
    /// Gets or sets the user that owns the token.
    /// </summary>
    public User User { get; set; } = null!;
}
