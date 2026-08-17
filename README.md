# Raycynix Identity

![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4.svg)
![Version](https://img.shields.io/badge/version-0.5.0-blue.svg)
![License](https://img.shields.io/badge/license-Apache--2.0-green.svg)

Raycynix Identity is the identity and access platform for Raycynix products. The project is being designed both as a
shared hosted identity service and as a self-hosted solution that organizations can configure and extend for their own
applications.

The current release provides account authentication, email confirmation, password recovery, JWT access tokens, secure
refresh-token rotation, session revocation, and PostgreSQL persistence. The feature-flagged OpenIddict server foundation
is present, but public OAuth 2.0 and OpenID Connect authorization flows are not operational yet.

## Current capabilities

- User registration with ASP.NET Core Identity.
- Login by username or email.
- Email confirmation and resend flows.
- Password-reset email delivery.
- JWT access-token issuance.
- Cryptographically random refresh tokens stored as HMAC-SHA-256 hashes.
- Refresh-token rotation, reuse detection, and active-session revocation.
- Secure `HttpOnly`, `Secure`, and `SameSite=Strict` refresh-token cookies.
- Identity lockout and configurable password requirements.
- Fixed-window rate limiting for sensitive authentication endpoints.
- Configurable cleanup of expired refresh tokens.
- SMTP email delivery with HTML templates.
- PostgreSQL persistence.
- Feature-flagged OpenIddict server and Entity Framework Core persistence foundation.
- Swagger UI in the Development environment.

## Project status

Raycynix Identity is under active development. The current HTTP API is an account and session service, not yet a
standards-compliant OAuth 2.0 authorization server or OpenID Connect provider.

The planned direction includes:

- Authorization and user-info handlers for the OpenIddict server foundation.
- Operational Authorization Code flow with PKCE.
- Standard discovery, JWKS, token, authorization, user-info, revocation, and logout endpoints.
- Application, redirect URI, scope, consent, and authorization management.
- Hosted multi-tenancy and a single-tenant self-hosted profile.
- Custom branding, claims, email templates, webhooks, and extension contracts.
- A separate worker process for outbox delivery and scheduled maintenance.
- Container and orchestration assets for self-hosted deployments.

## Solution layout

```text
Raycynix.Identity.sln
├─ src/
│  ├─ Raycynix.Identity.Host/
│  └─ Raycynix.Identity.Abstractions/
├─ orchestration/
│  └─ Raycynix.Identity.AppHost/
├─ workers/
├─ deployments/
└─ tests/
   └─ Raycynix.Identity.Host.Tests/
```

| Project                          | Responsibility                                                                                                       |
|----------------------------------|----------------------------------------------------------------------------------------------------------------------|
| `Raycynix.Identity.Host`         | ASP.NET Core host, account API, application logic, Identity persistence, and the current background cleanup service. |
| `Raycynix.Identity.Abstractions` | Standalone NuGet package containing public extension contracts without dependencies on Host internals.               |
| `Raycynix.Identity.AppHost`      | Aspire orchestration entry point. Resource definitions are still being added.                                        |
| `Raycynix.Identity.Host.Tests`   | Unit and contract tests for the current Host implementation.                                                         |

The `workers` and `deployments` solution areas are reserved for the future worker host and self-hosted deployment
assets.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL
- An SMTP server for account emails

## Build

Clone the repository and build the solution:

```shell
git clone https://github.com/raycynix-official/Identity.git
cd Identity
dotnet restore
dotnet build Raycynix.Identity.sln
```

## Configuration

Configuration is read from the standard ASP.NET Core configuration providers and Raycynix configuration extensions. Do
not commit production secrets.

At minimum, provide PostgreSQL connection settings, JWT settings, and SMTP credentials through environment variables,
user secrets, or an environment-specific configuration provider.

### Database

```json
{
  "DatabaseConfiguration": {
    "ConnectionConfiguration": {
      "Host": "localhost",
      "Port": 5432,
      "Name": "raycynix_identity",
      "Username": "postgres",
      "Password": "<database-password>"
    },
    "UseMigrations": false,
    "EnsureCreated": true
  }
}
```

The current development configuration uses `EnsureCreated`. A migration-based production database lifecycle is planned
before the self-hosted distribution is considered production-ready.

### JWT and refresh-token hashing

```json
{
  "SecurityConfiguration": {
    "Jwt": {
      "Authority": "https://id.example.com",
      "Issuer": "https://id.example.com",
      "Audience": "example-api",
      "AccessTokenLifetime": "00:15:00",
      "RefreshTokenLifetime": "14.00:00:00",
      "ClockSkew": "00:01:00",
      "RequireHttpsMetadata": true,
      "Secret": "<at-least-32-byte-jwt-secret>"
    },
    "RefreshTokenHashSecret": "<optional-dedicated-hmac-secret>"
  }
}
```

`SecurityConfiguration:RefreshTokenHashSecret` is optional. When omitted, the JWT secret is used for refresh-token HMAC
hashing. A dedicated secret is recommended.

Password-reset emails link to `ResetPasswordOptions:PageUrl`. Root-relative values are resolved against
`SecurityConfiguration:Jwt:Authority`; an absolute HTTP or HTTPS URL can be used when the account UI is hosted
separately. The target page must collect the new password and submit the email, token, and password to
`POST /api/v1/auth/password-reset/reset`.

### OAuth 2.0 and OpenID Connect foundation

OpenIddict is disabled by default and can be enabled through `OpenIddictOptions`. The current foundation registers the
authorization, token, logout, revocation, and user-info endpoint URIs, persists OpenIddict applications,
authorizations, scopes, and tokens in PostgreSQL, enables Authorization Code and Refresh Token flows, and requires PKCE.

Development uses generated development certificates:

```json
{
  "OpenIddictOptions": {
    "Enabled": true,
    "Issuer": "https://localhost:7000/",
    "UseDevelopmentCertificates": true
  }
}
```

Development certificates are rejected outside the Development environment. Production requires separate PKCS#12
certificates containing private keys:

```json
{
  "OpenIddictOptions": {
    "Enabled": true,
    "Issuer": "https://id.example.com/",
    "UseDevelopmentCertificates": false,
    "SigningCertificatePath": "certificates/signing.pfx",
    "SigningCertificatePassword": "<from-secret-provider>",
    "EncryptionCertificatePath": "certificates/encryption.pfx",
    "EncryptionCertificatePassword": "<from-secret-provider>"
  }
}
```

Certificate passwords should be supplied by environment variables or another secret-backed configuration provider, not
committed to configuration files. The environment variable names are
`OpenIddictOptions__SigningCertificatePassword` and `OpenIddictOptions__EncryptionCertificatePassword`.

This foundation PR does not yet implement the login/consent authorization handler, user-info response handler, client
provisioning UI, or production database migrations. Enabling the feature advertises the protocol endpoints, but complete
authorization requests will be supported by the subsequent protocol PRs.

### SMTP

```json
{
  "EmailConfiguration": {
    "DefaultFromAddress": "no-reply@example.com",
    "DefaultFromDisplayName": "Example Identity",
    "SmtpConfiguration": {
      "Host": "smtp.example.com",
      "Port": 465,
      "SecureSocketOptions": "SslOnConnect",
      "Username": "smtp-user",
      "Password": "<smtp-password>",
      "TimeoutMilliseconds": 100000
    }
  }
}
```

### Identity and security policies

The Host validates these configuration sections during startup:

- `EmailConfirmationOptions`
- `ResetPasswordOptions`
- `RateLimitOptions`
- `RefreshTokenRevocationOptions`
- `BackgroundServiceOptions`
- `IdentityOptions`
- `OpenIddictOptions` when the server feature is enabled

Default values are defined in [appsettings.json](src/Raycynix.Identity.Host/appsettings.json).

Environment variables use standard ASP.NET Core double-underscore notation:

```text
DatabaseConfiguration__ConnectionConfiguration__Host
DatabaseConfiguration__ConnectionConfiguration__Password
SecurityConfiguration__Jwt__Authority
SecurityConfiguration__Jwt__Issuer
SecurityConfiguration__Jwt__Audience
SecurityConfiguration__Jwt__Secret
SecurityConfiguration__RefreshTokenHashSecret
EmailConfiguration__SmtpConfiguration__Username
EmailConfiguration__SmtpConfiguration__Password
```

## Run the Host

After configuring PostgreSQL, JWT, and SMTP settings:

```shell
dotnet run --project src/Raycynix.Identity.Host/Raycynix.Identity.Host.csproj
```

In the Development environment, the root path redirects to Swagger UI at `/swagger`.

The Aspire AppHost currently provides the orchestration project foundation but does not yet register the Identity Host
or PostgreSQL as resources.

## Account API

Base route: `/api/v1/auth`

| Method | Route                         | Description                                                                     |
|--------|-------------------------------|---------------------------------------------------------------------------------|
| `POST` | `/registration`               | Registers a user and sends an email confirmation link.                          |
| `POST` | `/login`                      | Authenticates a user, returns an access token, and sets a refresh-token cookie. |
| `POST` | `/email-confirmation/send`    | Sends a new email confirmation link.                                            |
| `GET`  | `/email-confirmation/confirm` | Confirms an email address from an email link.                                   |
| `POST` | `/email-confirmation/confirm` | Confirms an email address from a request body.                                  |
| `POST` | `/password-reset/send`        | Sends a password-reset link.                                                    |
| `POST` | `/password-reset/reset`       | Resets a password using an Identity token.                                      |
| `POST` | `/logout`                     | Revokes the current refresh token and removes its cookie.                       |
| `POST` | `/refresh`                    | Rotates the refresh token and returns a new access token.                       |

These routes are the current Raycynix account API. They are not OAuth 2.0 or OpenID Connect protocol endpoints.

## Authentication and session behavior

- Registration creates an ASP.NET Core Identity user and sends an email-confirmation link.
- Login can require a confirmed email through `IdentityOptions:SignIn:RequireConfirmedEmail`.
- Access tokens are short-lived JWT bearer tokens.
- Refresh tokens are issued through a secure HTTP-only cookie scoped to `/api/v1/auth`.
- Only refresh-token hashes are persisted.
- Each successful refresh rotates and revokes the previous token.
- Reuse of a revoked refresh token can revoke all active refresh tokens belonging to the affected user.
- A successful password reset can revoke all active refresh tokens.
- Expired refresh tokens are removed by `RefreshTokensCleanupBackground`.

## Extension abstractions

`Raycynix.Identity.Abstractions` is prepared as a standalone NuGet package for self-hosted extensions. After publication to a configured NuGet feed, consumers can install it with:

```shell
dotnet add package Raycynix.Identity.Abstractions
```

The initial preview API contains `IClaimsContributor` and `ClaimsContributionContext`. It has no dependency on
`Raycynix.Identity.Host` or other Raycynix packages.

Package-specific documentation:

- [Abstractions README](src/Raycynix.Identity.Abstractions/README.md)
- [Abstractions changelog](src/Raycynix.Identity.Abstractions/CHANGELOG.md)

The package currently inherits version `0.5.0` from `Directory.Build.props`. Host integration for claims contributors
will be added as the OAuth/OIDC principal-building pipeline is implemented.

Build the package locally with:

```shell
dotnet pack src/Raycynix.Identity.Abstractions/Raycynix.Identity.Abstractions.csproj -c Release
```

## Tests

Run the current test project with:

```shell
dotnet test tests/Raycynix.Identity.Host.Tests/Raycynix.Identity.Host.Tests.csproj
```

Or test the complete solution:

```shell
dotnet test Raycynix.Identity.sln
```

## Technology

- .NET 10 and ASP.NET Core
- ASP.NET Core Identity
- OpenIddict
- Entity Framework Core
- PostgreSQL
- .NET Aspire
- NUnit
- Swagger / OpenAPI
- Raycynix Extensions

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for product changes. The Abstractions package maintains its own release history.

## License

Raycynix Identity is licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE).
