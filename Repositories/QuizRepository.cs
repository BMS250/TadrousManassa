using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class QuizRepository(ApplicationDbContext context) : IQuizRepository
    {
        private readonly ApplicationDbContext _context = context;


        public Task<string?> GetQuizIdByVideoIdAsync(string videoId)
        {
            return _context.Videos
                .AsNoTracking()
                .Where(v => v.Id == videoId)
                .Select(v => v.QuizId)
                .FirstOrDefaultAsync();
        }
        public async Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id);
        }
        
        public async Task<QuizDetailsDTO?> GetQuizDetailsAsync(string id)
        {
            return await _context.Quizzes
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name ?? "Quiz",
                    Description = q.Description ?? string.Empty,
                    TimeHours = q.TimeHours,
                    TimeMinutes = q.TimeMinutes
                })
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
