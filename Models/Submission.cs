using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Submission
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StudentQuizId { get; set; }

        [ForeignKey("StudentQuizId")]
        public StudentQuiz StudentQuiz { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime SubmissionTime { get; set; }
        public float Score { get; set; }
        public int OrderOfSubmission { get; set; }
    }
}
