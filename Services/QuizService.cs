using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IStudentQuizRepository _studentQuizRepository;
        public QuizService(IQuizRepository quizRepository, IStudentQuizRepository studentQuizRepository)
        {
            _quizRepository = quizRepository;
            _studentQuizRepository = studentQuizRepository;
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

        public async Task<QuizResultDTO?> GetQuizResultAsync(string studentId, string quizId, int remainingAttempts, Dictionary<string, string> answers)
        {
            var studentScore = await _studentQuizRepository.GetStudentScoreAsync(studentId, quizId);
            var quizResult = await _quizRepository.GetQuizResultAsync(studentId, quizId, remainingAttempts, answers);

            if (quizResult != null)
            {
                quizResult.BestScore = studentScore;
            }

            return quizResult;
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
