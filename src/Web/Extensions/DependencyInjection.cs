// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Identity;
using Raycynix.Extensions.Common.Context;
using Raycynix.Extensions.Configuration;
using Raycynix.Extensions.Database.AspNetCore.Identity;
using Raycynix.Extensions.Database.PostgreSql;
using Raycynix.Extensions.Email;
using Raycynix.Extensions.Email.Smtp;
using Raycynix.Extensions.Exceptions;
using Raycynix.Extensions.Logging;
using Raycynix.Extensions.Secrets;
using Raycynix.Extensions.Security.AspNetCore;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Domain.Configurations;
using Raycynix.Services.AuthService.Domain.Configurations.BackgroundServices;
using Raycynix.Services.AuthService.Domain.Configurations.Validators;
using Raycynix.Services.AuthService.Domain.Entities.Identity;
using Raycynix.Services.AuthService.Web.Background;

namespace Raycynix.Services.AuthService.Web.Extensions;

/// <summary>
/// Provides dependency-injection registration for the service.
/// </summary>
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers application services, database integrations, Identity, logging, secrets, and exception handling.
        /// </summary>
        /// <param name="configuration">The application configuration source.</param>
        /// <returns>The configured service collection.</returns>
        public IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddRaycynixExceptions();

            services.AddRaycynixConfiguration<BackgroundServiceConfiguration>(
                configuration,
                requireSection: true);
            services
                .AddRaycynixConfigurationValidator<BackgroundServiceConfiguration,
                    BackgroundServiceConfigurationValidator>();
            
            services.AddRaycynixConfiguration<EmailConfirmationConfiguration>(
                configuration,
                requireSection: true);
            services.AddRaycynixConfigurationValidator<EmailConfirmationConfiguration,
                EmailConfirmationConfigurationValidator>();
            
            services.AddRaycynixConfiguration<ResetPasswordConfiguration>(configuration, requireSection: true);
            services
                .AddRaycynixConfigurationValidator<ResetPasswordConfiguration, ResetPasswordConfigurationValidator>();

            services.AddScoped<IOperationContext, OperationContext>();

            services.AddRaycynixLogging();
            services.AddRaycynixEmail(configuration).AddSmtp();
            services.Configure<JwtConfiguration>(configuration.GetSection("SecurityConfiguration:Jwt"));

            services
                .AddRaycynixIdentityDatabase<
                    RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim,
                        UserToken>
                >(configuration).AddPostgreSql();
            services.AddRaycynixSecrets();
            services.AddRaycynixAspNetCoreSecurity(configuration);

            services.AddIdentity<User, Role>(
                    options => configuration.GetSection(nameof(IdentityOptions)).Bind(options))
                .AddEntityFrameworkStores<
                    RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim,
                        UserToken>
                >()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, Application.Services.AuthService>();

            var backgroundServicesConfiguration = configuration.GetSection(nameof(BackgroundServiceConfiguration))
                .Get<BackgroundServiceConfiguration>();
            if (backgroundServicesConfiguration is null)
                throw new InvalidOperationException("Background services configuration not found");

            if (!backgroundServicesConfiguration.RefreshTokensCleanupEnabled) return services;

            services.AddHostedService<RefreshTokensCleanupBackground>();

            return services;
        }
    }
}
