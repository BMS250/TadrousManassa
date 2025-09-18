using TadrousManassa.Models;

namespace TadrousManassa.Areas.Student.Models
{
    public class SolveQuizVM
    {
        public string QuizId { get; set; }
        public string QuizName { get; set; }
        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }
        public DateTime QuizStartTime { get; set; }
        public List<QuestionVM> Questions { get; set; } = new();
    }
}
