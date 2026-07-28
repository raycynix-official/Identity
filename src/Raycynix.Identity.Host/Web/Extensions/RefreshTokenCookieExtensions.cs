// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Identity.Host.Web.Extensions;

/// <summary>
/// Provides helpers for reading and writing refresh-token cookies.
/// </summary>
public static class RefreshTokenCookieExtensions
{
    /// <summary>
    /// The name of the refresh-token cookie.
    /// </summary>
    private const string RefreshTokenCookieName = "refresh_token";

    extension(IResponseCookies cookies)
    {
        /// <summary>
        /// Appends the refresh token cookie with secure options and the specified lifetime.
        /// </summary>
        /// <param name="refreshToken">The raw refresh token value.</param>
        /// <param name="lifetime">The cookie lifetime.</param>
        public void AppendRefreshToken(string refreshToken, TimeSpan lifetime)
        {
            cookies.Append(RefreshTokenCookieName, refreshToken, CreateRefreshTokenCookieOptions(lifetime));
        }

        /// <summary>
        /// Deletes the refresh token cookie using the same cookie options used when it is created.
        /// </summary>
        public void DeleteRefreshToken()
        {
            cookies.Delete(RefreshTokenCookieName, CreateRefreshTokenCookieOptions());
        }
    }

    extension(IRequestCookieCollection cookies)
    {
        /// <summary>
        /// Reads the refresh token from the request cookies.
        /// </summary>
        /// <returns>The raw refresh token value, or <see langword="null"/> when the cookie is missing.</returns>
        public string? GetRefreshToken() => cookies[RefreshTokenCookieName];
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(TimeSpan? lifetime = null)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
            IsEssential = true
        };

        if (lifetime is not null)
        {
            options.Expires = DateTimeOffset.UtcNow.Add(lifetime.Value);
            options.MaxAge = lifetime.Value;
        }

        return options;
    }
}
