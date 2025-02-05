using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //[MaxLength(70)]
        //public override string? UserName { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        //[MaxLength(70)]
        //public override string? Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[MaxLength(255)]
        //public string PasswordHash { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //[StringLength(11)]
        //public string PhoneNumber { get; set; }

        public virtual Student Student { get; set; }
    }
}
