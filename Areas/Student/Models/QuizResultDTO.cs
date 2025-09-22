namespace TadrousManassa.Areas.Student.Models
{
    public class QuizResultDTO
    {
        public string LectureId { get; set; }
        public string QuizId { get; set; }
        public string? QuizTitle { get; set; }
        public List<QuestionResultDTO> Questions { get; set; }
        public int RemainingAttempts { get; set; }
        public float TotalScore { get; set; }
        public float? CurrentScore { get; set; }
        public float? BestScore { get; set; }
        public bool IsSuccess => BestScore >= 0.5 * TotalScore;
        public bool IsNextVideoExists { get; set; }
        public int? NextVideoOrder { get; set; }
    }
}
