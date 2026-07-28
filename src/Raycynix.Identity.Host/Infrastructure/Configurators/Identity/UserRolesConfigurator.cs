// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Infrastructure.Configurators.Identity;

/// <summary>
/// Configures the user-role assignment entity mapping.
/// </summary>
[DatabaseTable("user_roles")]
public class UserRolesConfigurator : GenericConfigurator<UserRole>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [typeof(User), typeof(Role)];

    /// <summary>
    /// Configures the user-role composite key, indexes, and user and role relationships.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);

        var entity = modelBuilder.Entity<UserRole>();

        entity.HasKey(x => new { x.UserId, x.RoleId });
        entity.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();

        entity.Property(x => x.UserId).IsRequired();
        entity
            .HasOne(x => x.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        entity.Property(x => x.RoleId).IsRequired();
        entity
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
