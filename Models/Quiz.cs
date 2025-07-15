using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Quiz
    {
        [Key]
        public string Id { get; set; }

        public string? Title => $"{Lecture?.Name} Quiz" ?? "Quiz";

        public string? Description { get; set; }

        public int TimeHours { get; set; } = 1;

        public int TimeMinutes { get; set; } = 0;

        [Required]
        public required string LectureId { get; set; }

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }

        public ICollection<Question> Questions { get; set; } = [];

        public virtual ICollection<StudentQuiz> StudentQuizzes { get; set; }
    }
}
