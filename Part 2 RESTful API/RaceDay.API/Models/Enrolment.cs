
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Enrolments")]
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        // Foreign keys
        public int ParticipantID { get; set; }

        public int EventCategoryID { get; set; }

        // Registration date
        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime EnrolmentDate { get; set; }

        // Registration status
        [Required]
        [Column(TypeName = "varchar(30)")]
        public string EventStatus { get; set; } = "Registered";

        // Payment status
        [Required]
        [Column(TypeName = "varchar(30)")]
        public string PaymentStatus { get; set; } = "Pending";

        // Payment details
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? PaymentReference { get; set; }

        // Unique registration confirmation code
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ConfirmationCode { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(ParticipantID))]
        public Participant? Participant { get; set; }

        [ForeignKey(nameof(EventCategoryID))]
        public EventCategory? EventCategory { get; set; }
    }
}

