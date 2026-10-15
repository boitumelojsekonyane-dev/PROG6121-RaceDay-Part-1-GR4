
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    /// <summary>
    /// Provides endpoints for viewing sporting event routes.
    /// </summary>
    [ApiController]
    [Route("api/events/{eventId:int}/routes")]
    public class RoutesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public RoutesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/routes
        [HttpGet]
        public async Task<IActionResult> GetRoutes(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    Message = "Event not found."
                });
            }

            var routes = await _context.Routes
                .AsNoTracking()
                .Where(r => r.EventID == eventId)
                .OrderBy(r => r.RouteID)
                .Select(r => new
                {
                    r.RouteID,
                    r.EventID,
                    r.AdminID,
                    r.RouteName,
                    r.Description,
                    r.RouteStartKm,
                    r.RouteFinishKm
                })
                .ToListAsync();

            return Ok(routes);
        }

        // GET: api/events/1/routes/2
        [HttpGet("{routeId:int}")]
        public async Task<IActionResult> GetRouteById(
            int eventId,
            int routeId)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .Where(r =>
                    r.EventID == eventId &&
                    r.RouteID == routeId)
                .Select(r => new
                {
                    r.RouteID,
                    r.EventID,
                    r.AdminID,
                    r.RouteName,
                    r.Description,
                    r.RouteStartKm,
                    r.RouteFinishKm
                })
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return NotFound(new
                {
                    Message = "Route not found for this event."
                });
            }

            return Ok(route);
        }
    }
}
