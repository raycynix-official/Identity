// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Raycynix.Extensions.Configuration.AspNetCore;
using Raycynix.Extensions.Database.AspNetCore;
using Raycynix.Extensions.Exceptions.AspNetCore;
using Raycynix.Extensions.Logging;
using Raycynix.Extensions.Security.AspNetCore;
using Raycynix.Extensions.Security.Configurations;
using Raycynix.Services.AuthService.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseRaycynixLogging();

builder.AddRaycynixAspNetCoreConfiguration();

builder.Services.AddServices(builder.Configuration);

builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new() { Title = "Auth API", Version = "v1" }); });

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("SecurityConfiguration").Get<JwtConfiguration>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
    };
});

var app = builder.Build();

app.MapControllers();

app.UseRaycynixExceptions();
app.UseRaycynixSecurity();
await app.InitializeRaycynixDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();