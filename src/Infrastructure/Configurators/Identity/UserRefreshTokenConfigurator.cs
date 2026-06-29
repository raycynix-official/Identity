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
/// Configures the refresh-token entity mapping.
/// </summary>
[DatabaseTable("user_refresh_tokens")]
public class UserRefreshTokenConfigurator : GenericConfigurator<UserRefreshToken>
{
    /// <inheritdoc />
    public override Type[] DependsOn => [typeof(User)];

    /// <summary>
    /// Configures the refresh-token table, key, indexes, fields, and user relationship.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);

        var entity = modelBuilder.Entity<UserRefreshToken>();

        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Id).IsUnique();
        entity.Property(x => x.Id).IsRequired();
        
        entity.Property(x => x.UserId).IsRequired();
        entity
            .HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasIndex(x => x.TokenHash).IsUnique();
        entity.Property(x => x.TokenHash).IsRequired();
        
        entity.Property(x => x.CreatedAt).IsRequired();
        entity.Property(x => x.ExpiresAt).IsRequired();
        
        entity.Property(x => x.RevokedAt).IsRequired(false);
        entity.Property(x => x.ReplacedByTokenHash).IsRequired(false);

        entity.Ignore(x => x.IsActive);
    }
}
