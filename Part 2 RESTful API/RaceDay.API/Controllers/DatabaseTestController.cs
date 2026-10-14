
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
/// <summary>   this is to ensure that the database does not create or modify any tables or records. </summary>  
namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/database-test")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public DatabaseTestController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            bool connected = await _context.Database.CanConnectAsync();

            if (connected)
            {
                return Ok(new
                {
                    message = "Successfully connected to RaceDayActivitiesDB!"
                });
            }

            return StatusCode(503, new
            {
                message = "Database connection failed."
            });
        }
    }
}
