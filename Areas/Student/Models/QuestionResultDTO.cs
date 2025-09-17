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
        public string? SelectedChoiceId { get; set; }
        public bool? IsCorrect { get; set; }

        // correct answer info
        public string? CorrectAnswerId { get; set; }
        public string? CorrectAnswerText { get; set; }
    }
}
