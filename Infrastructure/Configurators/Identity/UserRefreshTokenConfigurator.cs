// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

public class UserRefreshTokenConfigurator : GenericConfigurator<UserRefreshToken>
{
    public override Type[] DependsOn => [typeof(User)];

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
        
        entity.Property(x => x.TokenHash).IsRequired();
        
        entity.Property(x => x.CreatedAt).IsRequired();
        entity.Property(x => x.ExpiresAt).IsRequired();
        
        entity.Property(x => x.RevokedAt).IsRequired(false);
        entity.Property(x => x.ReplacedByTokenHash).IsRequired(false);

        entity.Ignore(x => x.IsActive);
    }
}