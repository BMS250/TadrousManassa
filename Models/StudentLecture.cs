using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class StudentLecture
    {
        public string Id { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Code { get; set; }

        [Required]
        public string LectureId { get; set; } = null!;

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }

        public string? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        public bool IsWatched { get; set; } = false;

        public int ViewsCount { get; set; } = 0;
    }
}
