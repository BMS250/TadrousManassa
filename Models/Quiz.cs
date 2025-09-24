using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Quiz
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        public string? Name => $"{Lecture?.Name} Quiz" ?? "Quiz";

        [MaxLength(255)]
        public string? Description { get; set; }

        public int TimeHours { get; set; } = 1;

        public int TimeMinutes { get; set; } = 0;

        [Required]
        public int NumOfQuestions { get; set; }

        public float TotalScore { get; set; }

        public int TotalNumOfAttempts { get; set; } = 2;

        [Required]
        public required string LectureId { get; set; }

        [ForeignKey("LectureId")]
        public Lecture Lecture { get; set; }

        [Required]
        public required string VideoId { get; set; }

        [ForeignKey("VideoId")]
        public Video Video { get; set; }

        public ICollection<Question> Questions { get; set; } = [];

        public virtual ICollection<StudentQuiz> StudentQuizzes { get; set; }
    }
}
