# Changelog

All notable changes to Raycynix Identity are documented in this file.

The standalone `Raycynix.Identity.Abstractions` package also maintains a package-specific changelog in `src/Raycynix.Identity.Abstractions/CHANGELOG.md`.

## [0.5.0]

### Added

- Added a feature-flagged OpenIddict OAuth 2.0 and OpenID Connect server foundation.
- Added Entity Framework Core persistence mappings for OpenIddict applications, authorizations, scopes, and tokens.
- Added Authorization Code and Refresh Token flow configuration with mandatory PKCE.
- Added development and production certificate configuration with startup validation.

## [0.4.0]

### Added

- Added the `Raycynix.Identity.AppHost` Aspire project as the orchestration foundation.
- Added the standalone `Raycynix.Identity.Abstractions` NuGet package.
- Added centralized solution metadata and version configuration through `Directory.Build.props`.
- Added dedicated solution areas for orchestration, future workers, and future deployment assets.
- Added `IClaimsContributor` and `ClaimsContributionContext` as the initial public extension contracts.
- Added package-specific README, changelog, release notes, tags, icon, license, and symbol package configuration for Abstractions.
- Added NUnit test coverage for controller endpoint attributes, auth route contracts, request validation, refresh-token cookies, refresh-token hashing, secret resolution, refresh-token entity behavior, and security configuration validation.

### Changed

- Renamed the product and solution from Raycynix Services AuthService to Raycynix Identity.
- Renamed the main application project to `Raycynix.Identity.Host`.
- Renamed the test project to `Raycynix.Identity.Host.Tests`.
- Updated namespaces and project references to the `Raycynix.Identity` naming scheme.
- Added centralized solution metadata and version configuration through `Directory.Build.props`.
- Added dedicated solution areas for orchestration, future workers, and future deployment assets.

### Fixed

- Fixed request DTO validation attributes so DataAnnotations are applied to generated record properties.

### Security

- Added typed configuration for refresh-token revocation policy behavior.
- Added HMAC-SHA-256 hashing for newly issued refresh tokens with legacy SHA-256 lookup compatibility.
- Added refresh-token revocation reasons, last-used timestamps, and revocation source token hashes.
- Added refresh-token reuse detection during refresh and login token rotation.
- Added active-session revocation when reuse of an already revoked refresh token is detected.
- Added refresh-token indexes for active session and revocation reason queries.

## [0.3.2]

### Added
* Added password reset email delivery with a configurable HTML template.
* Added reset-password configuration validation.

### Changed
* Password reset token requests now send reset links by email instead of returning tokens in API responses.
* Password reset links now use encoded Identity tokens and the reset endpoint route constants.
* Updated password reset API documentation, README endpoint documentation, and email template wording.

### Security
* Added fixed-window rate limiting for sensitive authentication endpoints.
* Added typed configuration and validation for authentication rate limits.
* Added startup validation for minimum JWT signing secret length.
* Disabled JWT bearer token persistence in authentication properties.
* Restricted refresh-token cookie scope and added explicit cookie max age.
* Removed provider error messages from account email delivery logs and exceptions.

## [0.3.1]

### Changed
* Moved the application project and source folders under the `src` directory.
* Updated the solution structure to nest the service project under the `src` solution folder.
* Updated Docker build paths for the new `src` layout.
* Updated XML documentation for background service configuration and cleanup dependencies.
* Updated README version, repository URL, TeamCity badge metadata, configuration documentation, project paths, and Swagger development behavior.
* Updated package documentation metadata paths for root-level README, changelog, license, and icon files.

## [0.3.0]

### Added
* Added file-based HTML template rendering for email confirmation messages.
* Added SMTP-backed email delivery for email confirmation links.
* Added configurable email confirmation enforcement through `IdentityOptions:SignIn:RequireConfirmedEmail`.
* Added email confirmation link generation and email confirmation endpoints.
* Added password reset token generation and password reset endpoints.
* Added confirmed-email enforcement for login.

### Changed
* Email confirmation links now use `SecurityConfiguration:Jwt:Authority` and route constants instead of a configured endpoint URL.
* Registration now sends an email confirmation link instead of issuing access and refresh tokens.
* Email confirmation token regeneration now sends a confirmation link instead of returning the token in the API response.
* Password reset now revokes the user's active refresh tokens after a successful reset.

## [0.2.1]

### Added
* Added configurable background cleanup for expired refresh tokens.
* Documented background refresh-token cleanup configuration.

### Changed
* Login now rotates an existing active refresh token for the authenticated user when the request includes a refresh-token cookie.
* Release builds no longer try to create a NuGet package for the service.

## [0.2.0]

### Added
* Added XML documentation for public application, web, domain, and infrastructure APIs.
* Documented authentication endpoints, refresh-token cookie behavior, and token rotation in the README.
* Added this changelog.

### Changed
* Updated README metadata and repository references for the current service.
* Documented the current local development flow with PostgreSQL from Docker Compose and the API running via `dotnet run`.

## [0.1.0]

### Added
* Initial Auth Service implementation.
* Added registration, login, logout, and refresh-token endpoints.
* Added ASP.NET Core Identity integration with PostgreSQL persistence.
* Added JWT access-token generation and refresh-token persistence.
