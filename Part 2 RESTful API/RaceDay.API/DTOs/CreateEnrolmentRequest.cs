
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateEnrolmentRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid event category.")]
        public int EventCategoryID { get; set; }
    }
}

