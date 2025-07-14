using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class QuestionService(QuestionRepository questionRepository) : IQuestionService
    {
        private readonly QuestionRepository _questionRepository = questionRepository;

        public async Task<Question?> GetQuestionByIdAsync(string questionId)
        {
            return await _questionRepository.GetQuestionByIdAsync(questionId);
        }

        public async Task<List<Question>> GetQuestionsByQuizIdAsync(string quizId)
        {
            return await _questionRepository.GetQuestionsByQuizIdAsync(quizId);
        }
        
        public async Task<List<Question>> GetQuestionsByLectureIdAsync(string lectureId)
        {
            return await _questionRepository.GetQuestionsByLectureIdAsync(lectureId);
        }

        public async Task AddQuestionToQuizAsync(Quiz quiz, Question question)
        {
            await _questionRepository.AddQuestionToQuizAsync(quiz, question);
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            await _questionRepository.UpdateQuestionAsync(question);
        }

        public async Task DeleteQuestionAsync(string questionId)
        {
            await _questionRepository.DeleteQuestionAsync(questionId);
        }
    }
}
