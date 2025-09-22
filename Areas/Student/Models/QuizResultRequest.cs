namespace TadrousManassa.Areas.Student.Models
{
    public class QuizResultRequest
    {
        public string? QuizId { get; set; }
        public float? CurrentScore { get; set; }
        public int? RemainingAttempts { get; set; }
        public Dictionary<string, string> Answers { get; set; } = new();
    }
}
