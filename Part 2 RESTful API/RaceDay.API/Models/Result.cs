
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Results")]
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        // Foreign key to Enrolments
        [Required]
        public int EnrolmentID { get; set; }

        // Participant positions
        public int? PositionCategory { get; set; }

        public int? OverallPosition { get; set; }

        // Pace in minutes per kilometre
        public decimal? PaceMinKm { get; set; }

        // Race timing
        [Column(TypeName = "time")]
        public TimeOnly? StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeOnly FinishTime { get; set; }

        // Administrator who captured the result
        [Required]
        public int CapturedByAdminID { get; set; }

        // Date and time the result was recorded
        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CapturedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(EnrolmentID))]
        public Enrolment? Enrolment { get; set; }

        [ForeignKey(nameof(CapturedByAdminID))]
        public Admin? Admin { get; set; }
    }
}

