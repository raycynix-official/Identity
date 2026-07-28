using Microsoft.AspNetCore.Identity;

namespace Raycynix.Identity.Host.Domain.Entities.Identity;

/// <summary>
/// Represents the assignment of a role to a user.
/// </summary>
public sealed class UserRole : IdentityUserRole<Guid>
{
    /// <summary>
    /// Gets or sets the assigned user.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the assigned role.
    /// </summary>
    public Role Role { get; set; } = null!;
}
