
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/auth-test")]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("participant")]
        [Authorize(Roles = "Participant")]
        public IActionResult ParticipantOnly()
        {
            var userId = User.FindFirstValue("sub");
            var participantId = User.FindFirstValue("participantId");

            return Ok(new
            {
                Message = "JWT authentication successful.",
                UserId = userId,
                ParticipantId = participantId,
                Role = "Participant"
            });
        }
    }
}

