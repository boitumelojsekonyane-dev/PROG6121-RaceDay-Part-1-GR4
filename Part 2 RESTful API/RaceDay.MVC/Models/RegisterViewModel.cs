
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

        // Registration form date selectors
        [Range(1, 31)]
        public int? BirthDay { get; set; }

        [Range(1, 12)]
        public int? BirthMonth { get; set; }

        public int? BirthYear { get; set; }

        // Identification choice
        public string? IdentificationType { get; set; }

        [StringLength(20)]
        [Display(Name = "South African ID Number")]
        public string? IdentityNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Passport Number")]
        public string? PassportNumber { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        // Used only when Other is selected
        [StringLength(20)]
        public string? OtherGender { get; set; }

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
            // Validate date of birth
            if (!BirthDay.HasValue ||
                !BirthMonth.HasValue ||
                !BirthYear.HasValue)
            {
                yield return new ValidationResult(
                    "Select your day, month and year of birth.",
                    new[] { nameof(BirthDay) });
            }
            else
            {
                int day = BirthDay.Value;
                int month = BirthMonth.Value;
                int year = BirthYear.Value;

                if (year < 1907 ||
                    year > DateTime.Today.Year ||
                    !IsValidDate(year, month, day))
                {
                    yield return new ValidationResult(
                        "Enter a valid date of birth.",
                        new[] { nameof(BirthDay) });
                }
                else
                {
                    var selectedDate =
                        new DateTime(year, month, day);

                    if (selectedDate > DateTime.Today)
                    {
                        yield return new ValidationResult(
                            "Date of birth cannot be in the future.",
                            new[] { nameof(BirthDay) });
                    }
                }
            }

            // Validate identification choice
            if (IdentificationType == "SouthAfricanID")
            {
                if (string.IsNullOrWhiteSpace(IdentityNumber))
                {
                    yield return new ValidationResult(
                        "Enter your South African ID number.",
                        new[] { nameof(IdentityNumber) });
                }
            }
            else if (IdentificationType == "Passport")
            {
                if (string.IsNullOrWhiteSpace(PassportNumber))
                {
                    yield return new ValidationResult(
                        "Enter your passport number.",
                        new[] { nameof(PassportNumber) });
                }
            }
            else
            {
                yield return new ValidationResult(
                    "Select an identification type.",
                    new[] { nameof(IdentificationType) });
            }

            // Validate custom gender
            if (Gender == "Other" &&
                string.IsNullOrWhiteSpace(OtherGender))
            {
                yield return new ValidationResult(
                    "Please specify your gender.",
                    new[] { nameof(OtherGender) });
            }
        }

        private static bool IsValidDate(
            int year, int month, int day)
        {
            if (year < 1 || year > 9999 ||
                month < 1 || month > 12)
            {
                return false;
            }

            return day >= 1 &&
                   day <= DateTime.DaysInMonth(year, month);
        }
    }
}
