
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;


namespace RaceDay.API.Controllers
{

    /// <summary> This controller is used to test database read operations. 
    /// It provides an endpoint to count the number of records in each table of the 
    /// RaceDay database without modifying the existing data. </summary>
    [ApiController]
    [Route("api/database-read-test")]
    public class DatabaseReadTestController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public DatabaseReadTestController(
            RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabaseRead()
        {
            try
            {
                var adminCount =
                    await _context.Admins.CountAsync();

                var participantCount =
                    await _context.Participants.CountAsync();

                var eventCount =
                    await _context.Events.CountAsync();

                var categoryCount =
                    await _context.EventCategories.CountAsync();

                var enrolmentCount =
                    await _context.Enrolments.CountAsync();

                var routeCount =
                    await _context.Routes.CountAsync();

                var weatherCount =
                    await _context.Weather.CountAsync();

                var resultCount =
                    await _context.Results.CountAsync();

                var eventTypeCount =
                    await _context.EventTypes.CountAsync();

                return Ok(new
                {
                    Message = "Database read successful!",
                    Admins = adminCount,
                    Participants = participantCount,
                    EventTypes = eventTypeCount,
                    Events = eventCount,
                    EventCategories = categoryCount,
                    Enrolments = enrolmentCount,
                    Routes = routeCount,
                    Weather = weatherCount,
                    Results = resultCount
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Message = "Database read failed. Check the application logs."
                });
            }
        }
      
[HttpGet("sample-records")]
        public async Task<IActionResult> GetSampleRecords()
        {
            try
            {
                var events = await _context.Events
                    .AsNoTracking()
                    .Select(e => new
                    {
                        e.EventID,
                        e.Title,
                        e.EventDate,
                        e.StartTime,
                        e.DistanceKm,
                        e.EntryFee,
                        e.StatusOfEvent
                    })
                    .Take(3)
                    .ToListAsync();

                var categories = await _context.EventCategories
                    .AsNoTracking()
                    .Select(c => new
                    {
                        c.EventCategoryID,
                        c.EventID,
                        c.CategoryName,
                        c.DistanceKm
                    })
                    .Take(3)
                    .ToListAsync();

                var enrolments = await _context.Enrolments
                    .AsNoTracking()
                    .Select(e => new
                    {
                        e.EnrolmentID,
                        e.ParticipantID,
                        e.EventCategoryID,
                        e.EnrolmentDate,
                        e.PaymentStatus,
                        e.AmountPaid
                    })
                    .Take(3)
                    .ToListAsync();

                return Ok(new
                {
                    Message = "Sample records retrieved successfully!",
                    Events = events,
                    Categories = categories,
                    Enrolments = enrolments
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Message = "Sample record retrieval failed. Check application logs."
                });
            }
        }

    }
}
