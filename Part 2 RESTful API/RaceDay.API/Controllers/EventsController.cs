
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

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


        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            // Get the Organiser's AdminID from their JWT.
            var adminIdClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                return Forbid();
            }

            // Confirm that the Organiser exists.
            var adminExists = await _context.Admins
                .AnyAsync(a => a.AdminID == adminId);

            if (!adminExists)
            {
                return Forbid();
            }

            // Confirm that the selected event type exists.
            var eventTypeExists = await _context.EventTypes
                .AnyAsync(t => t.EventTypeID == request.EventTypeID);

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    Message = "The selected event type does not exist."
                });
            }

            // Get today's date in South African time.
            var southAfricaTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time");

            var southAfricaNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, southAfricaTimeZone);

            var today = DateOnly.FromDateTime(southAfricaNow);

            if (request.EventDate <= today)
            {
                return BadRequest(new
                {
                    Message = "The event date must be in the future."
                });
            }

            if (request.RegistrationOpens > request.RegistrationCloses)
            {
                return BadRequest(new
                {
                    Message = "Registration opening date cannot be after the closing date."
                });
            }

            if (request.RegistrationCloses >= request.EventDate)
            {
                return BadRequest(new
                {
                    Message = "Registration must close before the event date."
                });
            }

            var newEvent = new Event
            {
                AdminID = adminId,
                EventTypeID = request.EventTypeID,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                EventDate = request.EventDate,
                StartTime = request.StartTime,
                Location = request.Location.Trim(),
                DistanceKm = request.DistanceKm,
                RegistrationOpens = request.RegistrationOpens,
                RegistrationCloses = request.RegistrationCloses,
                EntryFee = request.EntryFee,
                StatusOfEvent = "Upcoming"
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventById),
                new { id = newEvent.EventID },
                new
                {
                    Message = "Event created successfully.",
                    newEvent.EventID,
                    newEvent.AdminID,
                    newEvent.Title,
                    newEvent.EventDate,
                    newEvent.StatusOfEvent
                });
        }


        // PUT: api/events/4
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] UpdateEventRequest request)
        {
            // Identify the logged-in Organiser.
            var adminIdClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                return Forbid();
            }

            // Find the existing event.
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    Message = "Event not found."
                });
            }

            // Organisers may edit only their own events.
            if (eventItem.AdminID != adminId)
            {
                return Forbid();
            }

            // Validate the selected event type.
            var eventTypeExists = await _context.EventTypes
                .AnyAsync(t => t.EventTypeID == request.EventTypeID);

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    Message = "The selected event type does not exist."
                });
            }

            // Validate dates using South African time.
            var southAfricaTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time");

            var southAfricaNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, southAfricaTimeZone);

            var today = DateOnly.FromDateTime(southAfricaNow);

            if (request.EventDate <= today)
            {
                return BadRequest(new
                {
                    Message = "The event date must be in the future."
                });
            }

            if (request.RegistrationOpens > request.RegistrationCloses)
            {
                return BadRequest(new
                {
                    Message = "Registration opening date cannot be after the closing date."
                });
            }

            if (request.RegistrationCloses >= request.EventDate)
            {
                return BadRequest(new
                {
                    Message = "Registration must close before the event date."
                });
            }

            // Validate text before saving.
            if (string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest(new
                {
                    Message = "Title, description and location cannot be empty."
                });
            }

            // Update only the permitted event properties.
            eventItem.EventTypeID = request.EventTypeID;
            eventItem.Title = request.Title.Trim();
            eventItem.Description = request.Description.Trim();
            eventItem.EventDate = request.EventDate;
            eventItem.StartTime = request.StartTime;
            eventItem.Location = request.Location.Trim();
            eventItem.DistanceKm = request.DistanceKm;
            eventItem.RegistrationOpens = request.RegistrationOpens;
            eventItem.RegistrationCloses = request.RegistrationCloses;
            eventItem.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event updated successfully.",
                eventItem.EventID,
                eventItem.AdminID,
                eventItem.Title,
                eventItem.EventDate,
                eventItem.EntryFee,
                eventItem.StatusOfEvent
            });
        }


        // DELETE: api/events/4
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // Identify the authenticated Organiser.
            var adminIdClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                return Forbid();
            }

            // Find the requested event.
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    Message = "Event not found."
                });
            }

            // An Organiser may delete only their own event.
            if (eventItem.AdminID != adminId)
            {
                return Forbid();
            }

            // Prevent deletion when related records exist.
            var hasCategories = await _context.EventCategories
                .AnyAsync(c => c.EventID == id);

            var hasRoutes = await _context.Routes
                .AnyAsync(r => r.EventID == id);

            var hasWeather = await _context.Weather
                .AnyAsync(w => w.EventID == id);

            if (hasCategories || hasRoutes || hasWeather)
            {
                return Conflict(new
                {
                    Message = "This event cannot be deleted because it has related categories, routes or weather records."
                });
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
