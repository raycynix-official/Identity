// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Frozen;
using System.Security.Claims;

namespace Raycynix.Identity.Abstractions.Claims;

/// <summary>
/// Provides identity and request information to a claims contributor.
/// </summary>
public sealed class ClaimsContributionContext
{
    /// <summary>
    /// Initializes a new claims contribution context.
    /// </summary>
    /// <param name="principal">The principal that will receive contributed claims.</param>
    /// <param name="subject">The stable subject identifier of the authenticated identity.</param>
    /// <param name="tenantId">The current tenant identifier, when the request is tenant-scoped.</param>
    /// <param name="clientId">The OAuth or OpenID Connect client identifier, when available.</param>
    /// <param name="scopes">The scopes granted for the current request.</param>
    public ClaimsContributionContext(
        ClaimsPrincipal principal,
        string subject,
        string? tenantId = null,
        string? clientId = null,
        IEnumerable<string>? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        Principal = principal;
        Subject = subject;
        TenantId = tenantId;
        ClientId = clientId;
        Scopes = (scopes ?? []).ToFrozenSet(StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets the principal that will receive contributed claims.
    /// </summary>
    public ClaimsPrincipal Principal { get; }

    /// <summary>
    /// Gets the stable subject identifier of the authenticated identity.
    /// </summary>
    public string Subject { get; }

    /// <summary>
    /// Gets the current tenant identifier, or <see langword="null"/> for a non-tenant request.
    /// </summary>
    public string? TenantId { get; }

    /// <summary>
    /// Gets the OAuth or OpenID Connect client identifier, when available.
    /// </summary>
    public string? ClientId { get; }

    /// <summary>
    /// Gets the scopes granted for the current request.
    /// </summary>
    public IReadOnlySet<string> Scopes { get; }
}
