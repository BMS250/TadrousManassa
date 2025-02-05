using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class StudentLecture
    {
        [Required]
        public string LectureId { get; set; } = null!;

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }     

        [Required]
        public string StudentId { get; set; } = null!;

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [Required]
        public int Code { get; set; }
    }
}
