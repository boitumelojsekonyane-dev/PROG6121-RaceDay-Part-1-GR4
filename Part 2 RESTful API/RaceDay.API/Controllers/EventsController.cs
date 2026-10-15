
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{

    /// <summary>
    /// The first endpoint will return a list of events ordered by their event dates
    /// the second endpoint retrieves a 404 NOT FOUND message if the event does not exist.
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventID,
                    e.Title,
                    e.Description,
                    e.EventDate,
                    e.StartTime,
                    e.Location,
                    e.DistanceKm,
                    e.EntryFee,
                    e.StatusOfEvent,
                    e.EventTypeID
                })
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.AdminID,
                    e.EventTypeID,
                    e.Title,
                    e.Description,
                    e.EventDate,
                    e.StartTime,
                    e.Location,
                    e.DistanceKm,
                    e.RegistrationOpens,
                    e.RegistrationCloses,
                    e.EntryFee,
                    e.StatusOfEvent
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new
                {
                    Message = "EVENT NOT FOUND."
                });
            }

            return Ok(eventItem);
        }
    }
}
