
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateEventViewModel
    {
        [Required]
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select an event type.")]
        [Display(Name = "Event Type")]
        public int EventTypeID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateOnly EventDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        [Display(Name = "Distance (km)")]
        public decimal DistanceKm { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Registration Opens")]
        public DateOnly RegistrationOpens { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Registration Closes")]
        public DateOnly RegistrationCloses { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Entry Fee (R)")]
        public decimal EntryFee { get; set; }
    }
}
