﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TadrousManassa.Utilities;

namespace TadrousManassa.Models
{
    public class Student
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        //[Required]
        //[MaxLength(70)]
        //public string Name { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        //[MaxLength(70)]
        //public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[MaxLength(255)]
        //public string PasswordHash { get; set; }

        //[Required]
        //[DataType (DataType.PhoneNumber)]
        //[StringLength(11)]
        //public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(11)]
        public string PhoneNumber_Parents { get; set; }

        [Required]
        [MaxLength(70)]
        public string Address { get; set; }

        [Required]
        [MaxLength(70)]
        public string School { get; set; }

        [Required]
        [AllowedValues(1, 2, 3, 5, 6)]
        public int Grade { get; set; }

        [Required]
        [MaxLength(255)]
        public string ReferralSource { get; set; }

        [Required]
        [MaxLength(255)]
        public string DeviceId { get; set; }

        public virtual ICollection<StudentLecture> StudentLectures { get; set; }
    }
}
