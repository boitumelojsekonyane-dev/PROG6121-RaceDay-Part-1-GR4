

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;


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


        // POST: api/enrolments
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] CreateEnrolmentRequest request)
        {
            // Identify the Participant from their JWT.
            var participantIdClaim =
                User.FindFirstValue("participantId");

            if (!int.TryParse(participantIdClaim, out int participantId))
            {
                return Unauthorized(new
                {
                    Message = "Valid Participant authentication is required."
                });
            }


            // Find the selected category and its associated event.
            var category = await _context.EventCategories
                .AsNoTracking()
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c =>
                    c.EventCategoryID == request.EventCategoryID);

            if (category == null)
            {
                return NotFound(new
                {
                    Message = "Event category not found."
                });
            }

            if (category.Event == null)
            {
                return NotFound(new
                {
                    Message = "The event associated with this category was not found."
                });
            }

            // Use South African local date for registration rules.
            var saTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                "South Africa Standard Time");

            var southAfricanNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, saTimeZone);

            var today = DateOnly.FromDateTime(southAfricanNow);

            // Only upcoming events accept registrations.
            if (!string.Equals(
                category.Event.StatusOfEvent,
                "Upcoming",
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    Message = "Registration is unavailable for this event."
                });
            }

            // Do not allow registration after the event date.
            if (today > category.Event.EventDate)
            {
                return BadRequest(new
                {
                    Message = "This event has already taken place."
                });
            }

            // Registration must fall within the permitted dates.
            if (today < category.Event.RegistrationOpens)
            {
                return BadRequest(new
                {
                    Message = "Registration for this event has not opened yet."
                });
            }

            if (today > category.Event.RegistrationCloses)
            {
                return BadRequest(new
                {
                    Message = "Registration for this event has closed."
                });
            }


            // Confirm that the selected category exists.
            var categoryExists = await _context.EventCategories
                .AnyAsync(c =>
                    c.EventCategoryID == request.EventCategoryID);

            if (!categoryExists)
            {
                return NotFound(new
                {
                    Message = "Event category not found."
                });
            }

            // Prevent duplicate enrolments in the same category.
            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.ParticipantID == participantId &&
                    e.EventCategoryID == request.EventCategoryID);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    Message = "You are already enrolled in this event category."
                });
            }

            // Generate a confirmation code.
            var confirmationCode =
                "RD-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();

            var enrolment = new Enrolment
            {
                ParticipantID = participantId,
                EventCategoryID = request.EventCategoryID,
                EnrolmentDate = DateTime.UtcNow,
                EventStatus = "Registered",
                PaymentStatus = "Pending",
                AmountPaid = 0m,
                ConfirmationCode = confirmationCode
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Event enrolment created successfully.",
                enrolment.EnrolmentID,
                enrolment.ParticipantID,
                enrolment.EventCategoryID,
                enrolment.ConfirmationCode,
                enrolment.PaymentStatus
            });
        }

    }
}
