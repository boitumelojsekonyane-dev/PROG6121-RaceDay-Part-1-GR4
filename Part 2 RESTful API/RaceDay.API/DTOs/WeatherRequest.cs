
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class WeatherRequest
    {
        [Required]
        public DateOnly? ForecastDate { get; set; }

        [Required]
        public TimeOnly? ForecastTime { get; set; }

        [Range(-100, 100)]
        public decimal TemperatureC { get; set; }

        [Range(0, 100)]
        public int RainProbability { get; set; }

        [Required]
        [StringLength(150)]
        public string Source { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

