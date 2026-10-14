
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("EventTypes")]
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string TypeName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(150)")]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
