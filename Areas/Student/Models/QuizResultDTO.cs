namespace TadrousManassa.Areas.Student.Models
{
    public class QuizResultDTO
    {
        public string QuizId { get; set; }
        public string? QuizTitle { get; set; }
        public List<QuestionResultDTO> Questions { get; set; }
        public int RemainingAttempts { get; set; }
    }
}
