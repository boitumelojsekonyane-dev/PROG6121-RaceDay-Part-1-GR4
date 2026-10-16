
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Identity;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ParticipantRegistrationService _registrationService;
        private readonly UserManager<RaceDayUser> _userManager;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(
            ParticipantRegistrationService registrationService,
            UserManager<RaceDayUser> userManager,
            JwtTokenService jwtTokenService)
        {
            _registrationService = registrationService;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _registrationService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Registration could not be completed. Please review your details and try again."
                });
            }

            return StatusCode(201, new
            {
                Message = "Participant registered successfully."
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var email = request.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            // Only participant accounts can use this login endpoint.
            if (!user.ParticipantID.HasValue ||
                !await _userManager.IsInRoleAsync(user, "Participant"))
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            var validPassword =
                await _userManager.CheckPasswordAsync(
                    user, request.Password);

            if (!validPassword)
            {
                await _userManager.AccessFailedAsync(user);

                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var token =
                await _jwtTokenService.GenerateTokenAsync(user);

            return Ok(new
            {
                Message = "Login successful.",
                Token = token,
                TokenType = "Bearer"
            });
        }
    }
}
