# Raycynix.Services.AuthService

![.NET Version](https://img.shields.io/badge/.NET-10.0-blue.svg)
![Version](https://img.shields.io/badge/version-0.0.1-green.svg)
![TeamCity build status](https://ci.raycynix.com/app/rest/builds/buildType:id:TMP_DotNet_GitHubDeploy/statusIcon.svg)

Auth Service for the Raycynix ecosystem, built with Clean Architecture principles and .NET 10.

## Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) for local PostgreSQL in containers
* [JetBrains Rider](https://www.jetbrains.com/rider/) or another IDE

### Run Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/Raycynix/Raycynix.Services.AuthService.git
   ```
2. Navigate to the project directory:
   ```bash
   cd Raycynix.Services.AuthService
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the application:
   ```bash
   dotnet run --project Raycynix.Services.AuthService.csproj
   ```

### Run With Docker Compose
For development, API and PostgreSQL can be started together:

```bash
docker compose up --build
```

Available endpoints:
* API: `http://localhost:5000`
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

## Project Structure

The solution follows Clean Architecture to ensure separation of concerns and testability:

* **Web**: The ASP.NET Core Web API entry point
* **Application**: Business logic, interfaces, DTOs
* **Domain**: Entities and domain rules
* **Infrastructure**: Database and external integrations

## Tech Stack
* **Framework:** ASP.NET Core (`net10.0`)
* **Database:** PostgreSQL
* **Architecture:** Clean Architecture / Onion Architecture

## License
This project is licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.
