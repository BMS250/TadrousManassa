namespace TadrousManassa.Areas.Teacher.Models
{
    public class QuestionCreatingVM
    {
        public string? Text { get; set; }
        public byte[]? Image { get; set; }
        public float Score { get; set; } = 1;
        public string AnswerId => Choices.FirstOrDefault(c => c.IsCorrect).Id;
        public string QuizId { get; set; }
        public List<ChoiceCreatingVM> Choices { get; set; } = [];
    }
}