
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Admins")]
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

