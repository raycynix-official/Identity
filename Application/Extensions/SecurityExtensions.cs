// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Raycynix.Extensions.Security.Abstractions.Interfaces;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Domain.Entities.Identity;

namespace Raycynix.Services.AuthService.Application.Extensions;

public static class SecurityExtensions
{
    extension(User user)
    {
        public async Task<JwtSecurityToken> GenerateTokenAsync(JwtConfiguration jwtSettings,
            ISecretResolver secretResolver)
        {
            return await user.GenerateTokenAsync(secretResolver, jwtSettings);
        }

        public async Task<JwtSecurityToken> GenerateTokenAsync(ISecretResolver secretResolver,
            JwtConfiguration jwtSettings)
        {
            var key = await secretResolver.GetSecurityKey();
            var credits = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(jwtSettings.AccessTokenLifetime),
                signingCredentials: credits
            );

            return token;
        }

        public UserRefreshToken GenerateUserRefreshToken(string refreshToken,
            JwtConfiguration jwtSettings)
        {
            var token = new UserRefreshToken
            {
                UserId = user.Id,
                TokenHash = HashRefreshToken(refreshToken),
                ExpiresAt = DateTimeOffset.UtcNow.Add(jwtSettings.RefreshTokenLifetime)
            };

            return token;
        }
    }

    public static UserRefreshToken GenerateUserRefreshToken(Guid userId, string refreshToken,
        JwtConfiguration jwtSettings)
    {
        var token = new UserRefreshToken
        {
            UserId = userId,
            TokenHash = HashRefreshToken(refreshToken),
            ExpiresAt = DateTimeOffset.UtcNow.Add(jwtSettings.RefreshTokenLifetime)
        };

        return token;
    }

    public static string TokenString(this JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    extension(ISecretResolver secretResolver)
    {
        public async Task<SymmetricSecurityKey> GetSecurityKey()
        {
            var secret = await secretResolver.GetJwtSecretAsync();
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public async Task<string> GetJwtSecretAsync()
        {
            var secret = await secretResolver.GetSecretAsync("SecurityConfiguration:Jwt:Secret");
            return secret ?? throw new InvalidOperationException("JWT Secret key not found");
        }
    }

    public static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }
}