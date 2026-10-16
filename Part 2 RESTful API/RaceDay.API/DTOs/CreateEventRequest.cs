
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateEventRequest
    {
        [Required]
        public int EventTypeID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateOnly EventDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal DistanceKm { get; set; }

        [Required]
        public DateOnly RegistrationOpens { get; set; }

        [Required]
        public DateOnly RegistrationCloses { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EntryFee { get; set; }
    }
}

