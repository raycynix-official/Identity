# Changelog

All notable changes to this project are documented in this file.

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
