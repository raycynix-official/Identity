# Raycynix.Services.AuthService

![.NET Version](https://img.shields.io/badge/.NET-10.0-blue.svg)
![Version](https://img.shields.io/badge/version-0.3.0-green.svg)
![TeamCity build status](https://ci.raycynix.com/app/rest/builds/buildType:id:TMP_DotNet_GitHubDeploy/statusIcon.svg)

Auth Service for the Raycynix ecosystem, built with ASP.NET Core, ASP.NET Core Identity, PostgreSQL, and .NET 10.

The service provides user registration, email confirmation, login, logout, refresh-token rotation, password reset, and expired refresh-token cleanup. Access tokens are returned in API responses, while refresh tokens are stored in secure HTTP-only cookies and persisted as SHA-256 hashes.

## Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) for local PostgreSQL in containers
* [JetBrains Rider](https://www.jetbrains.com/rider/) or another IDE

### Run Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/Raycynix/Services.AuthService.git
   ```
2. Navigate to the project directory:
   ```bash
   cd Services.AuthService
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Start PostgreSQL:
   ```bash
   docker compose up -d postgres
   ```
5. Configure the JWT secret and database credentials for your environment.
6. Run the application:
   ```bash
   dotnet run --project Raycynix.Services.AuthService.csproj
   ```

### Run With Docker Compose
For development, PostgreSQL can be started with Docker Compose:

```bash
docker compose up -d postgres
```

Available endpoints:
* PostgreSQL: `localhost:5432`

PostgreSQL credentials from `docker-compose.yml`:
* database: `raycynix_auth_dev`
* username: `postgres`
* password: `postgres`

To stop containers:

```bash
docker compose down
```

To remove the database volume too:

```bash
docker compose down -v
```

## API

Base route: `/api/v1/auth`

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/registration` | Registers a user and sends an email confirmation link. |
| `POST` | `/login` | Authenticates by username or email, returns an access token, and sets a refresh-token cookie. |
| `POST` | `/email-confirmation/send` | Sends a new email confirmation link. |
| `GET` | `/email-confirmation/confirm` | Confirms a user's email address from an email confirmation link. |
| `POST` | `/email-confirmation/confirm` | Confirms a user's email address. |
| `POST` | `/password-reset/token` | Generates a password reset token. |
| `POST` | `/password-reset/reset` | Resets a user's password. |
| `POST` | `/logout` | Revokes the active refresh token and deletes the refresh-token cookie. |
| `POST` | `/refresh` | Rotates the active refresh token and returns a new access token. |

Swagger UI is available at `/swagger` in the Development environment.

## Authentication Flow

* Access tokens are JWT bearer tokens signed with the configured JWT secret.
* Registration creates a user, generates an ASP.NET Core Identity email confirmation token, and sends a confirmation link by email.
* Login requires a confirmed email address when `IdentityOptions:SignIn:RequireConfirmedEmail` is enabled.
* Refresh tokens are generated from cryptographically random bytes.
* Refresh tokens are stored in the database as SHA-256 hashes.
* Refresh-token cookies are `HttpOnly`, `Secure`, and `SameSite=Strict`.
* Login reads the existing refresh-token cookie, revokes the active token for the authenticated user, and links it to the newly issued refresh token.
* Refreshing a token revokes the previous refresh token and links it to the replacement token hash.
* Password reset uses ASP.NET Core Identity password reset tokens and revokes the user's active refresh tokens after a successful reset.

## Email Configuration

Account emails are sent through `Raycynix.Extensions.Email.Smtp`.

```json
"EmailConfiguration": {
  "DefaultFromAddress": "no-reply@raycynix.com",
  "DefaultFromDisplayName": "Raycynix Auth",
  "SmtpConfiguration": {
    "Host": "smtp.example.com",
    "Port": 465,
    "SecureSocketOptions": "SslOnConnect",
    "Username": "smtp-user",
    "Password": "smtp-password",
    "TimeoutMilliseconds": 100000
  }
}
```

Email confirmation links are built from `EmailConfirmationConfiguration`.

```json
"EmailConfirmationConfiguration": {
  "ConfirmationUrl": "https://auth.raycynix.com/api/v1/auth/email-confirmation/confirm"
}
```

## Identity Options

Identity behavior is controlled through the standard ASP.NET Core Identity options.

```json
"IdentityOptions": {
  "SignIn": {
    "RequireConfirmedEmail": false,
    "RequireConfirmedPhoneNumber": false
  }
}
```

When `SignIn:RequireConfirmedEmail` is `true`, users must confirm their email address before login. When it is `false`, email confirmation tokens can still be generated and confirmed, but login does not require confirmation.

## Background Services

Expired refresh tokens are removed by `RefreshTokensCleanupBackground`. The service is controlled through `BackgroundServicesConfiguration` and validated through Raycynix typed configuration.

```json
"BackgroundServicesConfiguration": {
  "RefreshTokensCleanupEnabled": true,
  "RefreshTokensCleanupInterval": "24:00:00"
}
```

When enabled, cleanup runs once on application start and then repeats after the configured interval.

## Project Structure

The solution follows Clean Architecture to ensure separation of concerns and testability:

* **Web**: The ASP.NET Core Web API entry point
* **Application**: Business logic, interfaces, DTOs
* **Domain**: Entities and domain rules
* **Infrastructure**: Database and external integrations

## Documentation

Public types and methods are documented with XML comments. Release builds generate the XML documentation file.

## Tech Stack
* **Framework:** ASP.NET Core (`net10.0`)
* **Identity:** ASP.NET Core Identity
* **Database:** PostgreSQL
* **Configuration:** Raycynix typed configuration
* **API Documentation:** Swagger / OpenAPI
* **Architecture:** Clean Architecture / Onion Architecture

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License
This project is licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.
