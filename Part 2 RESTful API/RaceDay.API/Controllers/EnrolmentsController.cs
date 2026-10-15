
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    /// <summary>
    /// Provides enrolment-related API operations.
    /// </summary>
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/enrolments/event/1/count
        [HttpGet("event/{eventId:int}/count")]
        public async Task<IActionResult> GetEnrolmentCountByEvent(
            int eventId)
        {
            var eventExists = await _context.Events
                .AsNoTracking()
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    Message = "Event not found."
                });
            }

            var enrolmentCount = await _context.Enrolments
                .AsNoTracking()
                .CountAsync(en =>
                    _context.EventCategories.Any(c =>
                        c.EventCategoryID == en.EventCategoryID &&
                        c.EventID == eventId));

            return Ok(new
            {
                EventID = eventId,
                TotalEnrolments = enrolmentCount
            });
        }
    }
}
