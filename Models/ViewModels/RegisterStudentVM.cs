using System.ComponentModel.DataAnnotations;
using TadrousManassa.Utilities;

namespace TadrousManassa.Models.ViewModels
{
    public class RegisterStudentVM
    {
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(70)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{11})?$", ErrorMessage = "Phone number must be 11 digits or empty.")]
        public string? PhoneNumber { get; set; } = "";

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{11})?$", ErrorMessage = "Phone number must be 11 digits or empty.")]
        public string? PhoneNumber_Parents { get; set; } = "";

        [MaxLength(70)]
        public string? Address { get; set; } = "";

        [MaxLength(70)]
        public string? School { get; set; } = "";

        [Required]
        [Range(1, 6, ErrorMessage = "Grade must be between 5 and 9.")]
        public int Grade { get; set; } = 1;

        [MaxLength(255)]
        public string? ReferralSource { get; set; } = "";
    }
}
