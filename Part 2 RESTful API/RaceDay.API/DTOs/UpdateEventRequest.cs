
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class UpdateEventRequest
    {
        [Range(1, int.MaxValue)]
        public int EventTypeID { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;

        public DateOnly EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal DistanceKm { get; set; }

        public DateOnly RegistrationOpens { get; set; }

        public DateOnly RegistrationCloses { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EntryFee { get; set; }
    }
}
