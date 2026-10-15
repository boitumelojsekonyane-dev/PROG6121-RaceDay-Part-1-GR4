
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    /// <summary>
    /// Provides read-only weather forecast information
    /// for RACEDAY sporting events.
    /// </summary>
    [ApiController]
    [Route("api/events/{eventId:int}/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public WeatherController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/weather
        [HttpGet]
        public async Task<IActionResult> GetWeatherByEvent(
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

            var forecasts = await _context.Weather
                .AsNoTracking()
                .Where(w => w.EventID == eventId)
                .OrderBy(w => w.ForecastDate)
                .ThenBy(w => w.ForecastTime)
                .Select(w => new
                {
                    w.WeatherID,
                    w.EventID,
                    w.ForecastDate,
                    w.ForecastTime,
                    w.TemperatureC,
                    w.RainProbability,
                    w.Source,
                    w.Notes,
                    w.CapturedAt
                })
                .ToListAsync();

            return Ok(forecasts);
        }

        // GET: api/events/1/weather/1
        [HttpGet("{weatherId:int}")]
        public async Task<IActionResult> GetWeatherById(
            int eventId,
            int weatherId)
        {
            var forecast = await _context.Weather
                .AsNoTracking()
                .Where(w =>
                    w.EventID == eventId &&
                    w.WeatherID == weatherId)
                .Select(w => new
                {
                    w.WeatherID,
                    w.EventID,
                    w.ForecastDate,
                    w.ForecastTime,
                    w.TemperatureC,
                    w.RainProbability,
                    w.Source,
                    w.Notes,
                    w.CapturedAt
                })
                .FirstOrDefaultAsync();

            if (forecast == null)
            {
                return NotFound(new
                {
                    Message = "Weather forecast not found for this event."
                });
            }

            return Ok(forecast);
        }
    }
}
