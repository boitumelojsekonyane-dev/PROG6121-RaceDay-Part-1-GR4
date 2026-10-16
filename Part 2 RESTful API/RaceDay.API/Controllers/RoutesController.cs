
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

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


        // POST: api/events/4/routes
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateRoute(
            int eventId,
            [FromBody] RouteRequest request)
        {
            if (!int.TryParse(
                User.FindFirst("adminId")?.Value, out int adminId))
                return Forbid();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.RouteName))
                return BadRequest(new { Message = "Route name is required." });

            if (request.RouteFinishKm <= request.RouteStartKm)
                return BadRequest(new
                {
                    Message = "Route finish distance must exceed start distance."
                });

            var route = new RaceDay.API.Models.Route
            {
                EventID = eventId,
                AdminID = adminId,
                RouteName = request.RouteName.Trim(),
                Description = request.Description?.Trim(),
                RouteStartKm = request.RouteStartKm,
                RouteFinishKm = request.RouteFinishKm
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRouteById),
                new { eventId, routeId = route.RouteID },
                new
                {
                    Message = "Route created successfully.",
                    route.RouteID,
                    route.EventID,
                    route.RouteName
                });
        }

        // PUT: api/events/4/routes/10
        [HttpPut("{routeId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateRoute(
            int eventId,
            int routeId,
            [FromBody] RouteRequest request)
        {
            if (!int.TryParse(
                User.FindFirst("adminId")?.Value, out int adminId))
                return Forbid();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            var route = await _context.Routes
                .FirstOrDefaultAsync(r =>
                    r.RouteID == routeId &&
                    r.EventID == eventId);

            if (route == null)
                return NotFound(new { Message = "Route not found." });

            if (string.IsNullOrWhiteSpace(request.RouteName))
                return BadRequest(new { Message = "Route name is required." });

            if (request.RouteFinishKm <= request.RouteStartKm)
                return BadRequest(new
                {
                    Message = "Route finish distance must exceed start distance."
                });

            route.RouteName = request.RouteName.Trim();
            route.Description = request.Description?.Trim();
            route.RouteStartKm = request.RouteStartKm;
            route.RouteFinishKm = request.RouteFinishKm;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Route updated successfully.",
                route.RouteID,
                route.EventID,
                route.RouteName
            });
        }

    }
}
