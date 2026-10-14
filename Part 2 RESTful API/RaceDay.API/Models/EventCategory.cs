
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("EventCategories")]
    public class EventCategory
    {
        [Key]
        public int EventCategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string CategoryName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(200)")]
        public string? Description { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public decimal? DistanceKm { get; set; }

        // Foreign key relationship with Events
        [ForeignKey(nameof(EventID))]
        public Event? Event { get; set; }
    }
}

