// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Infrastructure.Configurators.Identity;

public class UserConfigurator : GenericConfigurator<User>
{
    public override Type[] DependsOn => [];

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.HasIndex(user => user.UserName)
            .IsUnique();
        builder.Property(user => user.UserName)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();
        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();
        
        builder.Property(user => user.LastLoginAt).IsRequired(false);
    }
}