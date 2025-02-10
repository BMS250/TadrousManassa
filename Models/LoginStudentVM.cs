using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class LoginStudentVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
