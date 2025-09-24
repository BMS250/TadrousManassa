namespace TadrousManassa.Areas.Teacher.Models
{
    public class QuizCreatingPartialVM
    {
        public int Grade { get; set; }
        public string LectureId { get; set; }
        public string VideoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }
        public int TotalNumOfAttempts { get; set; } = 2;
        public float TotalScore => Questions.Sum(q => q.Score);
        public int NumOfQuestions => Questions.Count;
        public List<QuestionCreatingVM> Questions { get; set; } = [];
    }
}
