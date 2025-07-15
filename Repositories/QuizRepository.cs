using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class QuizRepository(ApplicationDbContext context) : IQuizRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        
        public async Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId)
        {
            return await _context.Quizzes
                .Where(q => q.LectureId == lectureId)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .ToListAsync();
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(string id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);

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
