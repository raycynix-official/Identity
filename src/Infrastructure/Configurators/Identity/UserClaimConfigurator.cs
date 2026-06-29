// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

/// <summary>
/// Configures the user-claim entity mapping.
/// </summary>
[DatabaseTable("user_claims")]
public class UserClaimConfigurator : GenericConfigurator<UserClaim>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [typeof(User)];

    /// <summary>
    /// Configures the user-claim table, key, indexes, and user relationship.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);

        var entity = modelBuilder.Entity<UserClaim>();

        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Id).IsUnique();
        entity.Property(x => x.Id).IsRequired();
        
        entity.Property(x => x.UserId).IsRequired();
        entity
            .HasOne(x => x.User)
            .WithMany(x => x.UserClaims)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
