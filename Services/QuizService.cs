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

        public Task<string?> GetQuizIdByVideoIdAsync(string videoId)
        {
            return _quizRepository.GetQuizIdByVideoIdAsync(videoId);
        }

        public async Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return await _quizRepository.GetQuizByIdAsync(id);
        }

        public async Task<QuizDetailsDTO?> GetQuizDetailsAsync(string id)
        {
            return await _quizRepository.GetQuizDetailsAsync(id);
        }

        public Task<OperationResult<string>> GetLectureIdByQuizId(string id)
        {
            return _quizRepository.GetLectureIdByQuizId(id);
        }

        public async Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId)
        {
            return await _quizRepository.GetQuizzesByLectureIdAsync(lectureId);
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            await _quizRepository.CreateQuizAsync(quiz);
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            await _quizRepository.UpdateQuizAsync(quiz);
        }

        public async Task DeleteQuizAsync(string id)
        {
            await _quizRepository.DeleteQuizAsync(id);
        }

    }
}
