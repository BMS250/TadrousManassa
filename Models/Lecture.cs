using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Models
{
    public class Lecture
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public int Grade { get; set; }
        
        [Required]
        public int Semester { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        [MaxLength(255)]
        public string VideoPath { get; set; }

        [MaxLength(255)]
        public string SheetPath { get; set; }

        [MaxLength(255)]
        public string QuizPath { get; set; }

        public virtual ICollection<StudentLecture> StudentLectures { get; set; }
    }
}
