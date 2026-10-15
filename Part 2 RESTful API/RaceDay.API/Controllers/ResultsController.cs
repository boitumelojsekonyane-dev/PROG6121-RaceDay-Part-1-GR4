
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

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
    }
}
