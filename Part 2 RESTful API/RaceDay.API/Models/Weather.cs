
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Weather")]
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }

        // Foreign key
        [Required]
        public int EventID { get; set; }

        // Weather forecast date and time
        [Column(TypeName = "date")]
        public DateOnly ForecastDate { get; set; }

        [Column(TypeName = "time")]
        public TimeOnly ForecastTime { get; set; }

        // Weather measurements
        public decimal TemperatureC { get; set; }

        [Range(0, 100)]
        public int RainProbability { get; set; }

        // Forecast source
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Source { get; set; } = string.Empty;

        // Additional weather information
        [Column(TypeName = "varchar(500)")]
        public string? Notes { get; set; }

        // Date and time the forecast was recorded
        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CapturedAt { get; set; }

        // Navigation property
        [ForeignKey(nameof(EventID))]
        public Event? Event { get; set; }
    }
}

