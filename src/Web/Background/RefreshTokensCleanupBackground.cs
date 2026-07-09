// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.EntityFrameworkCore;
using Raycynix.Extensions.Configuration.Abstractions.Interfaces;
using Raycynix.Extensions.Database.AspNetCore.Identity;
using Raycynix.Services.AuthService.Domain.Configurations;
using Raycynix.Services.AuthService.Domain.Configurations.BackgroundServices;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Web.Background;

/// <summary>
/// Periodically removes expired refresh tokens from the database.
/// </summary>
/// <param name="logger">The logger used to write cleanup events.</param>
/// <param name="backgroundServicesConfiguration">The accessor that provides background service configuration.</param>
/// <param name="serviceScopeFactory">The factory used to create scopes for database cleanup work.</param>
public class RefreshTokensCleanupBackground(
    Raycynix.Extensions.Logging.Abstractions.ILogger<RefreshTokensCleanupBackground> logger,
    IConfigurationAccessor<BackgroundServiceOptions> backgroundServicesConfiguration,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configuration = backgroundServicesConfiguration.Current;
        if (!configuration.RefreshTokensCleanupEnabled)
        {
            logger.LogInformation("Refresh tokens cleanup background service is disabled");
            return;
        }

        logger.LogInformation("Refresh tokens cleanup background service started");
        await CleanUpExpiredTokensAsync(stoppingToken);

        using var timer = new PeriodicTimer(configuration.RefreshTokensCleanupInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanUpExpiredTokensAsync(stoppingToken);
        }
    }

    private async Task CleanUpExpiredTokensAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Cleaning up expired refresh tokens");

        using var scope = serviceScopeFactory.CreateScope();
        var databaseContext = scope.ServiceProvider
            .GetRequiredService<
                RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
            >();

        var countDeletedExpiredTokens = await databaseContext
            .Set<UserRefreshToken>()
            .Where(token => token.ExpiresAt < DateTimeOffset.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted {count} expired refresh tokens", countDeletedExpiredTokens);
    }
}
