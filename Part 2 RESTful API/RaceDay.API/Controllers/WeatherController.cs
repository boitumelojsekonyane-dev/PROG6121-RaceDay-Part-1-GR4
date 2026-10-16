
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

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


        // POST: api/events/4/weather
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateWeather(
            int eventId,
            [FromBody] WeatherRequest request)
        {
            if (!int.TryParse(
                User.FindFirst("adminId")?.Value, out int adminId))
            {
                return Forbid();
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            if (!request.ForecastDate.HasValue ||
                !request.ForecastTime.HasValue)
            {
                return BadRequest(new
                {
                    Message = "Forecast date and time are required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Source))
            {
                return BadRequest(new
                {
                    Message = "Weather forecast source is required."
                });
            }

            var forecast = new Weather
            {
                EventID = eventId,
                ForecastDate = request.ForecastDate.Value,
                ForecastTime = request.ForecastTime.Value,
                TemperatureC = request.TemperatureC,
                RainProbability = request.RainProbability,
                Source = request.Source.Trim(),
                Notes = request.Notes?.Trim(),
                CapturedAt = DateTime.UtcNow
            };

            _context.Weather.Add(forecast);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetWeatherById),
                new
                {
                    eventId,
                    weatherId = forecast.WeatherID
                },
                new
                {
                    Message = "Weather forecast created successfully.",
                    forecast.WeatherID,
                    forecast.EventID,
                    forecast.ForecastDate,
                    forecast.ForecastTime,
                    forecast.TemperatureC,
                    forecast.RainProbability,
                    forecast.Source
                });
        }

        // PUT: api/events/4/weather/10
        [HttpPut("{weatherId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateWeather(
            int eventId,
            int weatherId,
            [FromBody] WeatherRequest request)
        {
            if (!int.TryParse(
                User.FindFirst("adminId")?.Value, out int adminId))
            {
                return Forbid();
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            var forecast = await _context.Weather
                .FirstOrDefaultAsync(w =>
                    w.WeatherID == weatherId &&
                    w.EventID == eventId);

            if (forecast == null)
                return NotFound(new { Message = "Weather forecast not found." });

            if (!request.ForecastDate.HasValue ||
                !request.ForecastTime.HasValue)
            {
                return BadRequest(new
                {
                    Message = "Forecast date and time are required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Source))
            {
                return BadRequest(new
                {
                    Message = "Weather forecast source is required."
                });
            }

            forecast.ForecastDate = request.ForecastDate.Value;
            forecast.ForecastTime = request.ForecastTime.Value;
            forecast.TemperatureC = request.TemperatureC;
            forecast.RainProbability = request.RainProbability;
            forecast.Source = request.Source.Trim();
            forecast.Notes = request.Notes?.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Weather forecast updated successfully.",
                forecast.WeatherID,
                forecast.EventID,
                forecast.ForecastDate,
                forecast.ForecastTime,
                forecast.TemperatureC,
                forecast.RainProbability,
                forecast.Source
            });
        }

    }
}
