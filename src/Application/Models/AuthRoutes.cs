// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Application.Models;

/// <summary>
/// Contains authentication endpoint route constants.
/// </summary>
public static class AuthRoutes
{
    /// <summary>
    /// The authentication API base route.
    /// </summary>
    public const string Base = "api/v1/auth";

    /// <summary>
    /// The registration route.
    /// </summary>
    public const string Registration = "registration";

    /// <summary>
    /// The login route.
    /// </summary>
    public const string Login = "login";

    /// <summary>
    /// The route used to send a new email confirmation link.
    /// </summary>
    public const string EmailConfirmationSend = "email-confirmation/send";

    /// <summary>
    /// The route used to confirm an email address.
    /// </summary>
    public const string EmailConfirmationConfirm = "email-confirmation/confirm";

    /// <summary>
    /// The absolute-path segment used for email confirmation links.
    /// </summary>
    public const string EmailConfirmationConfirmPath = $"{Base}/{EmailConfirmationConfirm}";

    /// <summary>
    /// The password reset token route.
    /// </summary>
    public const string PasswordResetToken = "password-reset/token";

    /// <summary>
    /// The password reset route.
    /// </summary>
    public const string PasswordReset = "password-reset/reset";

    /// <summary>
    /// The logout route.
    /// </summary>
    public const string Logout = "logout";

    /// <summary>
    /// The refresh-token route.
    /// </summary>
    public const string Refresh = "refresh";
}
