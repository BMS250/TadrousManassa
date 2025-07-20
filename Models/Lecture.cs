using System.ComponentModel.DataAnnotations;
using TadrousManassa.Utilities;

namespace TadrousManassa.Models
{
    public class Lecture
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public int Grade { get; set; }
        
        [Required]
        public int Semester { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public bool UsedThisYear { get; set; } = true;

        [MaxLength(75)]
        public string? Unit { get; set; }

        [Required]
        public int Price { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [MaxLength(255)]
        public string? SheetPath { get; set; }

        public int ViewsCount { get; set; } = 0;

        public virtual ICollection<StudentLecture> StudentLectures { get; set; }

        public virtual List<Video> Videos { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
