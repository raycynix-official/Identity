using Microsoft.AspNetCore.Identity;

namespace Raycynix.Identity.Host.Domain.Entities.Identity;

/// <summary>
/// Represents a claim assigned to a role.
/// </summary>
public class RoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    /// Gets or sets the role that owns the claim.
    /// </summary>
    public Role Role { get; set; } = null!;
}
