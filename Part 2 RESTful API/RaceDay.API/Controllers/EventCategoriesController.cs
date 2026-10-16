
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/categories")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventCategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventId)
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

            var categories = await _context.EventCategories
                .AsNoTracking()
                .Where(c => c.EventID == eventId)
                .OrderBy(c => c.EventCategoryID)
                .Select(c => new
                {
                    c.EventCategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.Description,
                    c.MinAge,
                    c.MaxAge,
                    c.DistanceKm
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/events/1/categories/2
        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById(
            int eventId,
            int categoryId)
        {
            var category = await _context.EventCategories
                .AsNoTracking()
                .Where(c =>
                    c.EventID == eventId &&
                    c.EventCategoryID == categoryId)
                .Select(c => new
                {
                    c.EventCategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.Description,
                    c.MinAge,
                    c.MaxAge,
                    c.DistanceKm
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new
                {
                    Message = "Category not found for this event."
                });
            }

            return Ok(category);
        }


        // POST: api/events/4/categories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            int eventId,
            [FromBody] CreateEventCategoryRequest request)
        {
            var adminClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminClaim, out int adminId))
                return Forbid();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { Message = "Category name is required." });

            if (request.MinAge.HasValue &&
                request.MaxAge.HasValue &&
                request.MinAge > request.MaxAge)
            {
                return BadRequest(new
                {
                    Message = "Minimum age cannot exceed maximum age."
                });
            }

            var category = new EventCategory
            {
                EventID = eventId,
                CategoryName = request.CategoryName.Trim(),
                Description = request.Description?.Trim(),
                MinAge = request.MinAge,
                MaxAge = request.MaxAge,
                DistanceKm = request.DistanceKm
            };

            _context.EventCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoryById),
                new
                {
                    eventId,
                    categoryId = category.EventCategoryID
                },
                new
                {
                    Message = "Category created successfully.",
                    category.EventCategoryID,
                    category.EventID,
                    category.CategoryName
                });
        }

        // PUT: api/events/4/categories/10
        [HttpPut("{categoryId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int eventId,
            int categoryId,
            [FromBody] CreateEventCategoryRequest request)
        {
            var adminClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminClaim, out int adminId))
                return Forbid();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            var category = await _context.EventCategories
                .FirstOrDefaultAsync(c =>
                    c.EventCategoryID == categoryId &&
                    c.EventID == eventId);

            if (category == null)
                return NotFound(new { Message = "Category not found." });

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return BadRequest(new { Message = "Category name is required." });

            if (request.MinAge.HasValue &&
                request.MaxAge.HasValue &&
                request.MinAge > request.MaxAge)
            {
                return BadRequest(new
                {
                    Message = "Minimum age cannot exceed maximum age."
                });
            }

            category.CategoryName = request.CategoryName.Trim();
            category.Description = request.Description?.Trim();
            category.MinAge = request.MinAge;
            category.MaxAge = request.MaxAge;
            category.DistanceKm = request.DistanceKm;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category updated successfully.",
                category.EventCategoryID,
                category.EventID,
                category.CategoryName
            });
        }

        // DELETE: api/events/4/categories/10
        [HttpDelete("{categoryId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(
            int eventId,
            int categoryId)
        {
            var adminClaim = User.FindFirst("adminId")?.Value;

            if (!int.TryParse(adminClaim, out int adminId))
                return Forbid();

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found." });

            if (eventItem.AdminID != adminId)
                return Forbid();

            var category = await _context.EventCategories
                .FirstOrDefaultAsync(c =>
                    c.EventCategoryID == categoryId &&
                    c.EventID == eventId);

            if (category == null)
                return NotFound(new { Message = "Category not found." });

            var hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.EventCategoryID == categoryId);

            if (hasEnrolments)
            {
                return Conflict(new
                {
                    Message = "Category cannot be deleted because participants are enrolled."
                });
            }

            _context.EventCategories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
