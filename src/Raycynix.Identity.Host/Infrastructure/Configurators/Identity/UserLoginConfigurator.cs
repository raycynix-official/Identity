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
/// Configures the external user-login entity mapping.
/// </summary>
[DatabaseTable("user_logins")]
public class UserLoginConfigurator : GenericConfigurator<UserLogin>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [typeof(User)];

    /// <summary>
    /// Configures the external login composite key, indexes, and user relationship.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);
        var entity = modelBuilder.Entity<UserLogin>();

        entity.HasKey(x => new { x.LoginProvider, x.ProviderKey });
        entity.HasIndex(x => new { x.UserId, x.LoginProvider, x.ProviderKey }).IsUnique();
        entity.Property(x => x.UserId).IsRequired();
        entity
            .HasOne(x => x.User)
            .WithMany(u => u.UserLogins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
