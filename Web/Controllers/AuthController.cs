// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Raycynix.Services.AuthService.Application.Interfaces;
using Raycynix.Services.AuthService.Application.Models;
using Raycynix.Services.AuthService.Domain.Entities;

namespace Raycynix.Services.AuthService.Web.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(UserManager<User> userManager, IAuthService authService) : ControllerBase
{
    
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterAsync([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request, cancellationToken);
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.LoginAsync(request, cancellationToken);
        return Ok(user);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(User, cancellationToken);
    }
}
