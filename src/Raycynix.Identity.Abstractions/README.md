# Raycynix.Identity.Abstractions

Public extension contracts for customizing Raycynix Identity without referencing its internal implementation assemblies.

The package is intended for extensions installed into a self-hosted Raycynix Identity deployment. It contains contracts and context models only; it does not contain the Identity host, persistence implementation, or protocol server.

## Installation

```shell
dotnet add package Raycynix.Identity.Abstractions
```

The package currently targets .NET 10.

## Claims contributors

Implement `IClaimsContributor` to add application-specific claims while Raycynix Identity creates a principal:

```csharp
using System.Security.Claims;
using Raycynix.Identity.Abstractions.Claims;

public sealed class DepartmentClaimsContributor : IClaimsContributor
{
    public ValueTask ContributeAsync(
        ClaimsContributionContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.Principal.Identity is ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim("department", "engineering"));
        }

        return ValueTask.CompletedTask;
    }
}
```

The Raycynix Identity host is responsible for registration and runtime discovery of extensions.

## Compatibility

`0.x` releases are previews. Public contracts may change between minor versions while the extension model is being established. Stable compatibility guarantees begin with version `1.0.0`.

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for package-specific release history.

## License

Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE).
