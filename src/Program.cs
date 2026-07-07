// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Raycynix.Extensions.Configuration.AspNetCore;
using Raycynix.Extensions.Database.AspNetCore;
using Raycynix.Extensions.Exceptions.AspNetCore;
using Raycynix.Extensions.Logging;
using Raycynix.Extensions.Security.AspNetCore;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Domain.Configurations;
using Raycynix.Services.AuthService.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseRaycynixLogging();

builder.AddRaycynixAspNetCoreConfiguration();

builder.Services.AddServices(builder.Configuration);

builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new() { Title = "Auth API", Version = "v1" }); });

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("SecurityConfiguration:Jwt").Get<JwtConfiguration>();
if (jwtSettings is null) throw new InvalidOperationException("JWT Configuration not found");

var jwtSecret = builder.Configuration["SecurityConfiguration:Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret)) throw new InvalidOperationException("JWT Secret key not found");
if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
    throw new InvalidOperationException("JWT Secret key must be at least 32 bytes long");

var rateLimitConfiguration = builder.Configuration.GetSection(nameof(RateLimitConfiguration))
    .Get<RateLimitConfiguration>();
if (rateLimitConfiguration is null) throw new InvalidOperationException("Rate limit configuration not found");
rateLimitConfiguration.Validate();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth-sensitive", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = rateLimitConfiguration.PermitLimit,
                Window = rateLimitConfiguration.Window,
                QueueLimit = rateLimitConfiguration.QueueLimit
            }));
});

builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = jwtSettings.ClockSkew
    };
});

var app = builder.Build();

app.UseRaycynixExceptions();
app.UseRaycynixSecurity();
app.UseRateLimiter();

app.MapControllers();

await app.InitializeRaycynixDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.Run();
