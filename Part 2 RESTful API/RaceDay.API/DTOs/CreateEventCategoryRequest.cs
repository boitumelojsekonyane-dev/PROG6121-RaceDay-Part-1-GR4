
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateEventCategoryRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Range(0, 120)]
        public int? MinAge { get; set; }

        [Range(0, 120)]
        public int? MaxAge { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? DistanceKm { get; set; }
    }
}

