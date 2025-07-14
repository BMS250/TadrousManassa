using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<List<Quiz>> GetQuizzesAsync(string lectureId)
        {
            return await _context.Quizzes
                .Where(q => q.LectureId == lectureId)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .ToListAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(string quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }

        public async Task<Quiz> GetQuizByLectureIdAsync(string lectureId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.LectureId == lectureId);
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public void AddQuestionToQuiz(Quiz quiz, Question question)
        {
            quiz.Questions.Add(question);
        }

        // update question
        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteQuizAsync(string quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz != null)
            {
                foreach (var question in quiz.Questions)
                {
                    _context.Choices.RemoveRange(question.Choices);
                }

                _context.Questions.RemoveRange(quiz.Questions);
                _context.Quizzes.Remove(quiz);

                await _context.SaveChangesAsync();
            }
        }

    }
}
