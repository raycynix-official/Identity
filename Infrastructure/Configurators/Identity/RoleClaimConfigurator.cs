using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

[DatabaseTable("role_claims")]
public class RoleClaimConfigurator : GenericConfigurator<RoleClaim>
{
    public override Type[] DependsOn => [typeof(Role)];

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