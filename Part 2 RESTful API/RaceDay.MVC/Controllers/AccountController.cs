
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authService;
        private readonly TokenValidationParameters _validationParameters;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            AuthApiService authService,
            TokenValidationParameters validationParameters,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _validationParameters = validationParameters;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate the registration form first.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Combine day, month and year into DateOfBirth.
            if (model.BirthDay.HasValue &&
                model.BirthMonth.HasValue &&
                model.BirthYear.HasValue)
            {
                model.DateOfBirth = new DateTime(
                    model.BirthYear.Value,
                    model.BirthMonth.Value,
                    model.BirthDay.Value);
            }

            // Transfer the custom gender value when selected.
            if (model.Gender == "Other")
            {
                model.Gender = model.OtherGender!.Trim();
            }

            // Keep only the selected identification document.
            if (model.IdentificationType == "SouthAfricanID")
            {
                model.IdentityNumber = model.IdentityNumber?.Trim();
                model.PassportNumber = null;
            }
            else if (model.IdentificationType == "Passport")
            {
                model.PassportNumber = model.PassportNumber?.Trim();
                model.IdentityNumber = null;
            }

            try
            {
                bool success = await _authService.RegisterAsync(model);

                if (!success)
                {
                    ModelState.AddModelError(
                        "",
                        "Registration could not be completed. " +
                        "Please check your details."
                    );

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Registration successful. Please log in.";

                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "API registration request failed."
                );

                ModelState.AddModelError(
                    "",
                    "Registration is temporarily unavailable."
                );

                return View(model);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                string? token =
                    await _authService.LoginAsync(model);

                if (string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError(
                        "",
                        "Invalid email, password or account type.");
                    return View(model);
                }

                var handler = new JwtSecurityTokenHandler();

                var principal = handler.ValidateToken(
                    token,
                    _validationParameters,
                    out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwt ||
                    jwt.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    throw new SecurityTokenException(
                        "Unexpected token signing algorithm.");
                }

                var role = principal.FindFirst(
                    "role")?.Value
                    ?? principal.FindFirst(
                        ClaimTypes.Role)?.Value;

                if (role != model.Role ||
                    (role != "Participant" &&
                     role != "Organiser"))
                {
                    ModelState.AddModelError(
                        "",
                        "Account role verification failed.");
                    return View(model);
                }

                var expiry = validatedToken.ValidTo;

                if (expiry <= DateTime.UtcNow)
                {
                    ModelState.AddModelError(
                        "",
                        "Your login session has expired.");
                    return View(model);
                }

                var identity = new ClaimsIdentity(
                    principal.Claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                var cookiePrincipal =
                    new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    cookiePrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = new DateTimeOffset(
                            expiry, TimeSpan.Zero),
                        AllowRefresh = false
                    });

                HttpContext.Session.SetString(
                    "AccessToken", token);

                HttpContext.Session.SetString(
                    "UserRole", role);

                return role == "Organiser"
                    ? RedirectToAction("Index", "Organiser")
                    : RedirectToAction("Index", "Participant");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex,
                    "JWT validation failed during login.");

                ModelState.AddModelError(
                    "",
                    "Authentication could not be verified.");

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "API login request failed.");

                ModelState.AddModelError(
                    "",
                    "Login is temporarily unavailable.");

                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
