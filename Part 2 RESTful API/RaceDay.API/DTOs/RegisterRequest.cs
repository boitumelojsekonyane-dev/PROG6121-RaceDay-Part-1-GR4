
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class RegisterRequest : IValidatableObject
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EmergencyContact { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        public string? IdentityNumber { get; set; }

        [StringLength(20)]
        public string? PassportNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Province { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(IdentityNumber) &&
                string.IsNullOrWhiteSpace(PassportNumber))
            {
                yield return new ValidationResult(
                    "Provide either an Identity Number or a Passport Number.",
                    new[] { nameof(IdentityNumber), nameof(PassportNumber) });
            }

            if (DateOfBirth == default || DateOfBirth.Date > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "Enter a valid date of birth.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}
