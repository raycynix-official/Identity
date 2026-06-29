using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

/// <summary>
/// Represents an application role.
/// </summary>
public sealed class Role : IdentityRole<Guid>
{
    /// <summary>
    /// Initializes a new empty role instance.
    /// </summary>
    public Role()
    {
    }

    /// <summary>
    /// Initializes a new role instance with the specified name.
    /// </summary>
    /// <param name="name">The role name.</param>
    public Role(string name) : base(name)
    {
    }

    /// <summary>
    /// Initializes a new role instance with a predefined identifier and name.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <param name="name">The role name.</param>
    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpper();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets or sets the users assigned to the role.
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the claims assigned to the role.
    /// </summary>
    public List<RoleClaim> RoleClaims { get; set; } = [];
}
