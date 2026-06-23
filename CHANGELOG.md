# Changelog

All notable changes to this project are documented in this file.

## [0.3.0]

### Added
* Added SMTP-backed email delivery for email confirmation links.
* Added configurable email confirmation enforcement through `IdentityOptions:SignIn:RequireConfirmedEmail`.
* Added email confirmation link generation and email confirmation endpoints.
* Added password reset token generation and password reset endpoints.
* Added confirmed-email enforcement for login.

### Changed
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
