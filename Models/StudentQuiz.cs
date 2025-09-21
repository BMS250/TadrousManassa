using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class StudentQuiz
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [Required]
        public string QuizId { get; set; }
        
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [Required]
        public int NumOfRemainingAttempts { get; set; } = 2;

        public bool? IsSuccess { get; set; } = null;
        public float? BestScore { get; set; } = null;

        // Navigation
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
