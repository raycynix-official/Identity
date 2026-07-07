// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

/// <summary>
/// Describes why a refresh token was revoked.
/// </summary>
public enum UserRefreshTokenRevocationReason
{
    /// <summary>
    /// The refresh token was revoked during logout.
    /// </summary>
    Logout = 1,

    /// <summary>
    /// The refresh token was replaced during login.
    /// </summary>
    LoginRotation = 2,

    /// <summary>
    /// The refresh token was replaced during refresh-token rotation.
    /// </summary>
    RefreshRotation = 3,

    /// <summary>
    /// The refresh token was revoked after a password reset.
    /// </summary>
    PasswordReset = 4,

    /// <summary>
    /// The refresh token was revoked after reuse of an already revoked token was detected.
    /// </summary>
    ReuseDetected = 5
}
