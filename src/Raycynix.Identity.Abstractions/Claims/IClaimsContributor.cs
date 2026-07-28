// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Identity.Abstractions.Claims;

/// <summary>
/// Contributes application-specific claims to an identity issued by Raycynix Identity.
/// </summary>
public interface IClaimsContributor
{
    /// <summary>
    /// Adds application-specific claims to the principal in the supplied context.
    /// </summary>
    /// <param name="context">The claims contribution context.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous contribution operation.</returns>
    ValueTask ContributeAsync(
        ClaimsContributionContext context,
        CancellationToken cancellationToken = default);
}
