
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int AdminID { get; set; }

        [Required]
        public int EventTypeID { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(500)")]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateOnly EventDate { get; set; }

        [Column(TypeName = "time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Location { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        [Column(TypeName = "date")]
        public DateOnly RegistrationOpens { get; set; }

        [Column(TypeName = "date")]
        public DateOnly RegistrationCloses { get; set; }

        public decimal EntryFee { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string StatusOfEvent { get; set; } = "UPCOMING!";

        // Navigation properties
        [ForeignKey(nameof(AdminID))]
        public Admin? Admin { get; set; }

        [ForeignKey(nameof(EventTypeID))]
        public EventType? EventType { get; set; }
    }
}
