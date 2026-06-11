// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.ComponentModel.DataAnnotations;

namespace Raycynix.Services.AuthService.Application.Models;

/// <summary>
/// Represents the data required to register a user.
/// </summary>
/// <param name="UserName">The unique user name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record RegisterRequest(
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Username is required.")]
    string UserName,
    
    [Required]
    [EmailAddress]
    string Email,
    
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Password is required.")]
    string Password);

/// <summary>
/// Represents the credentials required to sign in.
/// </summary>
/// <param name="Login">The user name or email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginRequest(
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Login is required.")]
    string Login,
    
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Password is required.")]
    string Password);
