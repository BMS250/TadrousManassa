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

        public float? Score1 { get; set; } = null;

        public DateTime? SubmissionTimeAttempt1 { get; set; } = null;

        public float? Score2 { get; set; } = null;
        
        public DateTime? SubmissionTimeAttempt2 { get; set; } = null;

        [Required]
        public int NumOfRemainingAttempts { get; set; } = 2;

        [Required]
        public DateTime StartTime { get; set; }

        public bool? IsSuccess { get; set; } = null;
    }
}
