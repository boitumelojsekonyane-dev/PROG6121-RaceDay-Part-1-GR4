
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class RouteRequest
    {
        [Required]
        [StringLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RouteStartKm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RouteFinishKm { get; set; }
    }
}

