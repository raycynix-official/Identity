using Microsoft.AspNetCore.Identity;

namespace Raycynix.Identity.Host.Domain.Entities.Identity;

/// <summary>
/// Represents an external login associated with a user.
/// </summary>
public sealed class UserLogin : IdentityUserLogin<Guid>
{
    /// <summary>
    /// Gets or sets the user that owns the external login.
    /// </summary>
    public User User { get; set; } = null!;
}
