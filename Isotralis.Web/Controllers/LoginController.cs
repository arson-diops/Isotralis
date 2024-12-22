﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Isotralis.Web.Models;
using Isotralis.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Isotralis.Web.Controllers;

[AllowAnonymous]
[Route("[controller]/[action]")]
public sealed class LoginController : Controller
{
    private readonly WindowsAuthenticationService _windowsAuthService;
    private readonly ILogger<LoginController> _logger;

    public LoginController(WindowsAuthenticationService was, ILogger<LoginController> logger)
    {
        _windowsAuthService = was;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogDebug("Entered GET LoginController::Index.");

        if (User.Identity?.IsAuthenticated ?? false)
        {
            _logger.LogInformation("User is authenticated, redirecting to home page.");
            return RedirectToAction("Index", "Home");
        }

        _logger.LogInformation("User is not authenticated, showing login page.");
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromForm] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state, returning to login page.");
            return View(model);
        }

        _logger.LogInformation("Attempting to authenticate user '{Username}'.", model.Username);

        if (!_windowsAuthService.TryAuthenticateUser(model.Username, model.Password, out UserInformation? userInformation))
        {
            _logger.LogWarning("Failed to authenticate user '{Username}'.", model.Username);
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        _logger.LogInformation("Successfully authenticated user '{Username}'. Creating authentication cookie.", model.Username);

        Claim[] claims = 
        [
            new(ClaimTypes.NameIdentifier, userInformation.Username),
            new(ClaimTypes.Name, userInformation.Name), 
            new(ClaimTypes.Role, Constants.TechnicianUserRole),
            new(ClaimTypes.Role, Constants.GeneralUserRole),
            ..userInformation.Roles.Select(static g => new Claim(ClaimTypes.Role, g))
        ];

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
        {
            AllowRefresh = true,
            IssuedUtc = DateTimeOffset.UtcNow,
            IsPersistent = true
        });

        _logger.LogInformation("User '{Username}' signed in, redirecting to home page.", model.Username);

        return RedirectToAction("Index", "Home");
    }
}