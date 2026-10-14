
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Participants")]
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string EmergencyContact { get; set; } = string.Empty;

        //the onlydate type is used to store only the date part without the time component, which is suitable for a date of birth field.
        [Required]
        [Column(TypeName = "date")]
        public DateOnly DateOfBirth { get; set; }

        //the ? mark indicates that the property can be null, which is useful for optional fields like IdentityNumber and PassportNumber.
        [Column(TypeName = "varchar(20)")]
        public string? IdentityNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? PassportNumber { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Province { get; set; } = string.Empty;


        //The database automatically generates the date and time when a new record is created, and this value cannot be modified afterward. The [DatabaseGenerated(DatabaseGeneratedOption.Computed)] attribute indicates that the value is computed by the database.
        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}
