// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

namespace Raycynix.Services.AuthService.Web.Extensions;

public static class RefreshTokenCookieExtensions
{
    public const string RefreshTokenCookieName = "refresh_token";

    extension(IResponseCookies cookies)
    {
        public void AppendRefreshToken(string refreshToken, TimeSpan lifetime)
        {
            cookies.Append(RefreshTokenCookieName, refreshToken, CreateRefreshTokenCookieOptions(lifetime));
        }

        public void DeleteRefreshToken()
        {
            cookies.Delete(RefreshTokenCookieName, CreateRefreshTokenCookieOptions());
        }
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(TimeSpan? lifetime = null)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        if (lifetime is not null)
        {
            options.Expires = DateTimeOffset.UtcNow.Add(lifetime.Value);
        }

        return options;
    }
}
