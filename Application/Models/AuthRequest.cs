// Copyright 2026 Raycynix
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

using System.ComponentModel.DataAnnotations;

namespace Raycynix.Services.AuthService.Application.Models;

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

public record LoginRequest(
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Login is required.")]
    string Login,
    
    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Password is required.")]
    string Password);
