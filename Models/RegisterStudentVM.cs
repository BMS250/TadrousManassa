using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
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

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber_Parents { get; set; }

        [Required]
        [MaxLength(70)]
        public string Address { get; set; }

        [Required]
        [MaxLength(70)]
        public string School { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        [MaxLength(255)]
        public string ReferralSource { get; set; }
    }
}
