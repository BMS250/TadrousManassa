namespace TadrousManassa.Areas.Student.Models
{
    public class QuizResultViewModel
    {
        public string QuizId { get; set; }
        public string QuizName { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
        public Dictionary<string, string> StudentAnswers { get; set; } = new Dictionary<string, string>();
    }
}
