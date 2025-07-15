using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class QuizService : IQuizService
    {
        private readonly QuizRepository _quizRepository;
        public QuizService(QuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return await _quizRepository.GetQuizByIdAsync(id);
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
