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
        public float Score1 { get; set; } = -1;

        [Required]
        public DateTime SubmissionTimeAttempt1 { get; set; }

        [Required]
        public float Score2 { get; set; } = -1;
        
        [Required]
        public DateTime SubmissionTimeAttempt2 { get; set; }

        [Required]
        public int NumOfRemainingAttempts { get; set; } = 2;

        [Required]
        public bool IsSuccess { get; set; }
    }
}
