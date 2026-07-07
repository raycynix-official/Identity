# Changelog

All notable changes to this project are documented in this file.

## [0.4.0]

### Security
* Added typed configuration for refresh-token revocation policy behavior.
* Added refresh-token revocation reasons, last-used timestamps, and revocation source token hashes.
* Added refresh-token reuse detection during refresh and login token rotation.
* Added active-session revocation when reuse of an already revoked refresh token is detected.
* Added refresh-token indexes for active session and revocation reason queries.

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
