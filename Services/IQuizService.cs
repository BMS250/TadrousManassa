using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public interface IQuizService
    {
        public List<Quiz> GetQuizzesAsync(string lectureId);
        public Quiz GetQuizByIdAsync(string quizId);
        public Quiz GetQuizByLectureIdAsync(string lectureId);
        public Quiz CreateQuizAsync(Quiz quiz);
        public Quiz UpdateQuizAsync(Quiz quiz);
        public Quiz DeleteQuizAsync(string quizId);
    }
}
