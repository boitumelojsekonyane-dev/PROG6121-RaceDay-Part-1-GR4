
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    /// <summary>
    /// Provides read-only access to sporting event results.
    /// </summary>
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/results/event/1
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetResultsByEvent(
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

            var results = await _context.Results
                .AsNoTracking()
                .Where(r =>
                    _context.Enrolments.Any(en =>
                        en.EnrolmentID == r.EnrolmentID &&
                        _context.EventCategories.Any(c =>
                            c.EventCategoryID == en.EventCategoryID &&
                            c.EventID == eventId)))
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.PositionCategory,
                    r.OverallPosition,
                    r.PaceMinKm,
                    r.StartTime,
                    r.FinishTime,
                    r.CapturedAt
                })
                .OrderBy(r => r.OverallPosition)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/results/participant/1
        [HttpGet("participant/{participantId:int}")]
        public async Task<IActionResult> GetResultsByParticipant(
            int participantId)
        {
            var participantExists = await _context.Participants
                .AsNoTracking()
                .AnyAsync(p => p.ParticipantID == participantId);

            if (!participantExists)
            {
                return NotFound(new
                {
                    Message = "Participant not found."
                });
            }

            var results = await _context.Results
                .AsNoTracking()
                .Where(r =>
                    _context.Enrolments.Any(en =>
                        en.EnrolmentID == r.EnrolmentID &&
                        en.ParticipantID == participantId))
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.PositionCategory,
                    r.OverallPosition,
                    r.PaceMinKm,
                    r.StartTime,
                    r.FinishTime,
                    r.CapturedAt
                })
                .OrderBy(r => r.CapturedAt)
                .ToListAsync();

            return Ok(results);
        }


        // POST: api/results
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(
            [FromBody] CreateResultRequest request)
        {
            var adminClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminClaim, out int adminId))
                return Forbid();

            if (!request.FinishTime.HasValue)
            {
                return BadRequest(new
                {
                    Message = "Finish time is required."
                });
            }

            // Locate the enrolment and its associated event.
            var enrolment = await _context.Enrolments
                .AsNoTracking()
                .Where(en => en.EnrolmentID == request.EnrolmentID)
                .Select(en => new
                {
                    en.EnrolmentID,
                    EventAdminID = _context.EventCategories
                        .Where(c => c.EventCategoryID == en.EventCategoryID)
                        .Join(
                            _context.Events,
                            c => c.EventID,
                            e => e.EventID,
                            (c, e) => e.AdminID)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found." });

            // An Organiser may only capture results for their own events.
            if (enrolment.EventAdminID != adminId)
                return Forbid();

            // Prevent multiple results for the same enrolment.
            bool resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentID == request.EnrolmentID);

            if (resultExists)
            {
                return Conflict(new
                {
                    Message = "A result already exists for this enrolment."
                });
            }

            var result = new Result
            {
                EnrolmentID = request.EnrolmentID,
                PositionCategory = request.PositionCategory,
                OverallPosition = request.OverallPosition,
                PaceMinKm = request.PaceMinKm,
                StartTime = request.StartTime,
                FinishTime = request.FinishTime.Value,
                CapturedByAdminID = adminId
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Result captured successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.PositionCategory,
                result.OverallPosition,
                result.FinishTime,
                result.CapturedByAdminID
            });
            
        }

    }
}
