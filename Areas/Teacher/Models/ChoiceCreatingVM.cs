namespace TadrousManassa.Areas.Teacher.Models
{
    public class ChoiceCreatingVM
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Text { get; set; }
        public byte[]? Image { get; set; }
        public bool IsCorrect { get; set; }
        public string QuizId { get; set; }
        public string QuestionId { get; set; }
    }
}