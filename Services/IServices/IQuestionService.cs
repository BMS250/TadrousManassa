using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public interface IQuestionService
    {
        public Task<Question?> GetQuestionByIdAsync(string questionId);
        public Task<List<Question>> GetQuestionsByQuizIdAsync(string quizId);
        public Task<List<Question>> GetQuestionsByLectureIdAsync(string lectureId);
        public Task AddQuestionToQuizAsync(Quiz quiz, Question question);
        public Task UpdateQuestionAsync(Question question);
        public Task DeleteQuestionAsync(string questionId);
    }
}
