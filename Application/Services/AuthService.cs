// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Raycynix.Extensions.Exceptions;
using Raycynix.Extensions.Security.Abstractions.Interfaces;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Domain.Entities;

namespace Raycynix.Services.AuthService.Application.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IOptions<JwtConfiguration> jwtSettings,
    ISecretResolver secretResolver,
    Extensions.Logging.Abstractions.ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = new User(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError(
                "User trying to register with username:{UserName} and email:{Email}.\nGet`s errors:{Errors}",
                request.UserName, request.Email, errors);
            throw new AuthenticationException(errors);
        }

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Email is null || request.UserName is null) throw new ArgumentException("Login is null");

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogError(
                "User trying to login with email:{Email} not found, trying to login with username:{UserName}",
                request.Email,
                request.UserName);

            user = await userManager.FindByNameAsync(request.UserName);
            if (user is null)
            {
                logger.LogError("User trying to login with username:{UserName} not found", request.UserName);
                throw new NotFoundException("User not found");
            }
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            logger.LogError(
                "User trying to login with email:{Email} or username:{UserName}. Get`s error: Invalid password",
                request.Email,
                request.UserName);
            throw new UnauthorizedAccessException("Invalid password");
        }

        return await GenerateTokenAsync(user);
    }

    public async Task LogoutAsync(ClaimsPrincipal userClaims, CancellationToken cancellationToken = default)
    {
        var userId = userClaims.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? userClaims.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning("Logout failed: user ID not found in claims.");
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        await signInManager.SignOutAsync();
    }

    private async Task<AuthResponse> GenerateTokenAsync(User user)
    {
        var jwtSecret = await GetJwtSecretAsync();
        if (jwtSecret is null) throw new AuthenticationException("JWT secret not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credits = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenLifetime),
            signingCredentials: credits
        );

        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }

    private async Task<string?> GetJwtSecretAsync() => await secretResolver.GetSecretAsync("SecurityConfiguration:Jwt:Secret");
}