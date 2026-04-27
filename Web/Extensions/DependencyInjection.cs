// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Common.Context;
using Raycynix.Extensions.Database;
using Raycynix.Extensions.Database.Configurations;
using Raycynix.Extensions.Database.Implementations;
using Raycynix.Extensions.Database.PostgreSql;
using Raycynix.Extensions.Exceptions;
using Raycynix.Extensions.Logging;
using Raycynix.Extensions.Secrets;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Web.Extensions;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddRaycynixExceptions();
            
            services.AddOptions<DatabaseConfiguration>()
                .Bind(configuration.GetSection("DatabaseConfiguration"));

            services.AddScoped<IOperationContext, OperationContext>();

            services.AddRaycynixLogging();

            services.AddRaycynixDatabase(configuration).AddPostgreSql();
            services.AddRaycynixSecrets();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, Application.Services.AuthService>();


            return services;
        }
    }
}