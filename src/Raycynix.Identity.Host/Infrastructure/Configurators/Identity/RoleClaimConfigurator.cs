using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Infrastructure.Configurators.Identity;

/// <summary>
/// Configures the role-claim entity mapping.
/// </summary>
[DatabaseTable("role_claims")]
public class RoleClaimConfigurator : GenericConfigurator<RoleClaim>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [typeof(Role)];

    /// <summary>
    /// Configures the role-claim table, key, indexes, and role relationship.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);
        var entity = modelBuilder.Entity<RoleClaim>();
        
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Id).IsUnique();
        
        entity
            .HasOne(x => x.Role)
            .WithMany(x => x.RoleClaims)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
