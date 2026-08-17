using System.Security.Cryptography.X509Certificates;
using OpenIddict.Abstractions;
using Raycynix.Extensions.Database.AspNetCore.Identity;
using Raycynix.Identity.Host.Domain.Configurations;
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Web.Extensions;

/// <summary>
/// Provides registration for the OAuth 2.0 and OpenID Connect server foundation.
/// </summary>
public static class OpenIddictDependencyInjection
{
    /// <summary>
    /// Registers OpenIddict persistence and server services when the feature is enabled.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The current hosting environment.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddRaycynixOpenIddict(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var section = configuration.GetSection(nameof(OpenIddictOptions));
        var settings = section.Get<OpenIddictOptions>() ?? new OpenIddictOptions();
        settings.Validate(environment.IsDevelopment());

        services.Configure<OpenIddictOptions>(section);
        if (!settings.Enabled)
        {
            return services;
        }

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<
                        RaycynixIdentityDatabaseContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim,
                            UserToken>>();
            })
            .AddServer(options =>
            {
                options.SetIssuer(new Uri(settings.Issuer!, UriKind.Absolute));

                options.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetEndSessionEndpointUris("/connect/logout")
                    .SetRevocationEndpointUris("/connect/revoke")
                    .SetUserInfoEndpointUris("/connect/userinfo");

                options.AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow()
                    .RequireProofKeyForCodeExchange();

                options.RegisterScopes(
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles);

                options.DisableAccessTokenEncryption();

                ConfigureCredentials(options, settings, environment);

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableEndSessionEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough();
            });

        return services;
    }

    private static void ConfigureCredentials(
        OpenIddictServerBuilder builder,
        OpenIddictOptions settings,
        IHostEnvironment environment)
    {
        if (settings.UseDevelopmentCertificates)
        {
            builder.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();
            return;
        }

        var signingCertificate = LoadCertificate(
            settings.SigningCertificatePath!,
            settings.SigningCertificatePassword,
            environment.ContentRootPath);
        var encryptionCertificate = LoadCertificate(
            settings.EncryptionCertificatePath!,
            settings.EncryptionCertificatePassword,
            environment.ContentRootPath);

        builder.AddSigningCertificate(signingCertificate)
            .AddEncryptionCertificate(encryptionCertificate);
    }

    private static X509Certificate2 LoadCertificate(string path, string? password, string contentRootPath)
    {
        var resolvedPath = Path.IsPathRooted(path)
            ? path
            : Path.Combine(contentRootPath, path);

        var certificate = X509CertificateLoader.LoadPkcs12FromFile(
            resolvedPath,
            password,
            X509KeyStorageFlags.EphemeralKeySet);

        if (!certificate.HasPrivateKey)
        {
            certificate.Dispose();
            throw new InvalidOperationException($"OpenIddict certificate '{path}' must contain a private key");
        }

        return certificate;
    }
}
