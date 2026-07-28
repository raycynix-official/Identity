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
using Raycynix.Identity.Host.Domain.Entities.Identity;

namespace Raycynix.Identity.Host.Application.Extensions;

/// <summary>
/// Provides helpers for JWT access tokens and refresh tokens.
/// </summary>
public static class SecurityExtensions
{
    extension(User user)
    {
        /// <summary>
        /// Generates a JWT access token for the user.
        /// </summary>
        /// <param name="jwtSettings">The JWT issuer, audience, and lifetime settings.</param>
        /// <param name="secretResolver">The resolver used to read the JWT signing secret.</param>
        /// <returns>The generated JWT access token.</returns>
        public async Task<JwtSecurityToken> GenerateTokenAsync(JwtConfiguration jwtSettings,
            ISecretResolver secretResolver)
        {
            return await user.GenerateTokenAsync(secretResolver, jwtSettings);
        }

        /// <summary>
        /// Generates a JWT access token for the user.
        /// </summary>
        /// <param name="secretResolver">The resolver used to read the JWT signing secret.</param>
        /// <param name="jwtSettings">The JWT issuer, audience, and lifetime settings.</param>
        /// <returns>The generated JWT access token.</returns>
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

        /// <summary>
        /// Creates a persisted refresh-token entity for the user from a refresh-token hash.
        /// </summary>
        /// <param name="tokenHash">The refresh-token hash stored by the service.</param>
        /// <param name="jwtSettings">The JWT settings that define the refresh-token lifetime.</param>
        /// <returns>The refresh-token entity with the token hash and expiration date set.</returns>
        public UserRefreshToken GenerateUserRefreshToken(string tokenHash,
            JwtConfiguration jwtSettings)
        {
            var token = new UserRefreshToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTimeOffset.UtcNow.Add(jwtSettings.RefreshTokenLifetime)
            };

            return token;
        }
    }

    /// <summary>
    /// Creates a persisted refresh-token entity for a user from a refresh-token hash.
    /// </summary>
    /// <param name="userId">The identifier of the user that owns the refresh token.</param>
    /// <param name="tokenHash">The refresh-token hash stored by the service.</param>
    /// <param name="jwtSettings">The JWT settings that define the refresh-token lifetime.</param>
    /// <returns>The refresh-token entity with the token hash and expiration date set.</returns>
    public static UserRefreshToken GenerateUserRefreshToken(Guid userId, string tokenHash,
        JwtConfiguration jwtSettings)
    {
        var token = new UserRefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(jwtSettings.RefreshTokenLifetime)
        };

        return token;
    }

    /// <summary>
    /// Serializes a JWT access token to its compact string representation.
    /// </summary>
    /// <param name="token">The token to serialize.</param>
    /// <returns>The serialized JWT string.</returns>
    public static string TokenString(this JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    extension(ISecretResolver secretResolver)
    {
        /// <summary>
        /// Builds a symmetric signing key from the configured JWT secret.
        /// </summary>
        /// <returns>The symmetric security key used to sign JWT access tokens.</returns>
        public async Task<SymmetricSecurityKey> GetSecurityKey()
        {
            var secret = await secretResolver.GetJwtSecretAsync();
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        /// <summary>
        /// Reads the JWT signing secret from the configured secret provider.
        /// </summary>
        /// <returns>The configured JWT signing secret.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JWT secret is not configured.</exception>
        public async Task<string> GetJwtSecretAsync()
        {
            var secret = await secretResolver.GetSecretAsync("SecurityConfiguration:Jwt:Secret");
            return secret ?? throw new InvalidOperationException("JWT Secret key not found");
        }

        /// <summary>
        /// Reads the refresh-token hashing secret from the configured secret provider.
        /// </summary>
        /// <returns>The configured refresh-token hashing secret, or the JWT secret when no dedicated secret is configured.</returns>
        public async Task<string> GetRefreshTokenHashSecretAsync()
        {
            var secret = await secretResolver.GetSecretAsync("SecurityConfiguration:RefreshTokenHashSecret");
            return string.IsNullOrWhiteSpace(secret)
                ? await secretResolver.GetJwtSecretAsync()
                : secret;
        }
    }

    /// <summary>
    /// Generates a cryptographically random refresh token.
    /// </summary>
    /// <returns>The generated refresh token encoded as Base64.</returns>
    public static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Hashes a raw refresh token before it is stored or compared.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token.</param>
    /// <param name="secret">The server-side HMAC secret.</param>
    /// <returns>The HMAC-SHA-256 hash of the refresh token encoded as Base64.</returns>
    public static string HashRefreshToken(string refreshToken, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        var bytes = HMACSHA256.HashData(secretBytes, tokenBytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Hashes a raw refresh token using the legacy unsalted SHA-256 format.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token.</param>
    /// <returns>The legacy SHA-256 hash of the refresh token encoded as Base64.</returns>
    public static string HashRefreshTokenLegacy(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }
}
