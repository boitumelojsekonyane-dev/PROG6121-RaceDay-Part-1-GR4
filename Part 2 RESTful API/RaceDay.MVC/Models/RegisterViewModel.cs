
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Emergency contact is required.")]
        [StringLength(100)]
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        [Display(Name = "South African ID Number")]
        public string? IdentityNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Passport Number")]
        public string? PassportNumber { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(50)]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),
            ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(IdentityNumber) &&
                string.IsNullOrWhiteSpace(PassportNumber))
            {
                yield return new ValidationResult(
                    "Provide either an Identity Number or a Passport Number.",
                    new[]
                    {
                        nameof(IdentityNumber),
                        nameof(PassportNumber)
                    });
            }

            if (DateOfBirth == default ||
                DateOfBirth.Date > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "Enter a valid date of birth.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}
