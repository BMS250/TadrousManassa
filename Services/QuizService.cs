using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return _quizRepository.GetQuizByIdAsync(id);
        }

        public Task<QuizDetailsDTO?> GetQuizDetailsAsync(string id)
        {
            return _quizRepository.GetQuizDetailsAsync(id);
        }

        public Task<OperationResult<string>> GetLectureIdByQuizId(string id)
        {
            return _quizRepository.GetLectureIdByQuizId(id);
        }

        public Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId)
        {
            return _quizRepository.GetQuizzesByLectureIdAsync(lectureId);
        }

        public Task<QuizResultDTO?> GetQuizResultAsync(string studentId, string quizId, int remainingAttempts)
        {
            return _quizRepository.GetQuizResultAsync(studentId, quizId, remainingAttempts);
        }

        public Task CreateQuizAsync(Quiz quiz)
        {
            return _quizRepository.CreateQuizAsync(quiz);
        }

        public Task UpdateQuizAsync(Quiz quiz)
        {
            return _quizRepository.UpdateQuizAsync(quiz);
        }

        public Task DeleteQuizAsync(string id)
        {
            return _quizRepository.DeleteQuizAsync(id);
        }

    }
}
