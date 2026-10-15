
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

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
    }
}
