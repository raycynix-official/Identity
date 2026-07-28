using Microsoft.AspNetCore.Identity;

namespace Raycynix.Identity.Host.Domain.Entities.Identity;

/// <summary>
/// Represents a claim assigned to a user.
/// </summary>
public sealed class UserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    /// Gets or sets the user that owns the claim.
    /// </summary>
    public User User { get; set; } = null!;
}
