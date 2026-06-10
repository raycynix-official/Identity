// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Database.Abstractions.Attributes;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

[DatabaseTable("users")]
public sealed class UserConfigurator : GenericConfigurator<User>
{
    public override Type[] DependsOn => [];

    public override void Configure(ModelBuilder modelBuilder)
    {
        base.Configure(modelBuilder);
        var entity = modelBuilder.Entity<User>();
        
        entity.HasKey(user => user.Id);

        entity.HasIndex(user => user.UserName)
            .IsUnique();
        entity.Property(user => user.UserName)
            .HasMaxLength(64)
            .IsRequired();

        entity.HasIndex(user => user.Email)
            .IsUnique();
        entity.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        entity.Property(user => user.CreatedAt)
            .IsRequired();
        
        entity.Property(user => user.LastLoginAt).IsRequired(false);
    }
}