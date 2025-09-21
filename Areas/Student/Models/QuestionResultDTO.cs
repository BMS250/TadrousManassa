using TadrousManassa.Models;

namespace TadrousManassa.Areas.Student.Models
{
    public class QuestionResultDTO
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string? QuestionImage { get; set; }
        public float Score { get; set; }

        public List<ChoiceResultDTO> Choices { get; set; }

        // student info
        public SummarizedStudentChoice? SummarizedStudentChoice { get; set; }
        public string? SelectedChoiceId { get; set; } /* => SummarizedStudentChoice?.ChoiceId;*/
        public bool? IsCorrect { get; set; }  /*=> SummarizedStudentChoice?.IsCorrect;*/

        // correct answer info
        public string? CorrectAnswerId { get; set; }
        public string? CorrectAnswerText { get; set; }
    }
}
