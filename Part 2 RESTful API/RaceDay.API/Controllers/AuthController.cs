
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ParticipantRegistrationService
            _registrationService;

        public AuthController(
            ParticipantRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

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
                // Do not expose account-existence details
                // through the public registration response.
                return BadRequest(new
                {
                    Message = "Registration could not be completed. " +
                              "Please review your details and try again."
                });
            }

            return StatusCode(201, new
            {
                Message = "Participant registered successfully."
            });
        }
    }
}
