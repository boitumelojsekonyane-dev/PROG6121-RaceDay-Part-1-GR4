
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Routes")]
    public class Route
    {
        [Key]
        public int RouteID { get; set; }

        // Foreign keys
        public int EventID { get; set; }

        public int AdminID { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string RouteName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        public string? Description { get; set; }

        public decimal RouteStartKm { get; set; }

        public decimal RouteFinishKm { get; set; }

        // Navigation properties
        [ForeignKey(nameof(EventID))]
        public Event? Event { get; set; }

        [ForeignKey(nameof(AdminID))]
        public Admin? Admin { get; set; }
    }
}
