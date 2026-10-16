
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateResultRequest
    {
        [Range(1, int.MaxValue)]
        public int EnrolmentID { get; set; }

        [Range(1, int.MaxValue)]
        public int? PositionCategory { get; set; }

        [Range(1, int.MaxValue)]
        public int? OverallPosition { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? PaceMinKm { get; set; }

        public TimeOnly? StartTime { get; set; }

        [Required]
        public TimeOnly? FinishTime { get; set; }
    }
}
