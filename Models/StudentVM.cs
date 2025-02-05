using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class StudentVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        public string DeviceId { get; set; }
    }
}
