using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class StudentQuizRepository : IStudentQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentQuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // The Problem here is because that this method is async where there is not ToHashSetAsync method in EF Core
        //public async Task<HashSet<string>> GetQuizIdsByStudentIdAsync(string studentId)
        //{
        //    return await _context.StudentQuizzes
        //        .Where(sq => sq.StudentId == studentId)
        //        .Select(sq => sq.QuizId)
        //        .ToHashSet();
        //}

        public async Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId)
        {
            return await _context.StudentQuizzes
                .Where(sq => sq.StudentId == studentId)
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Choices)
                .Select(sq => sq.Quiz)
                .ToListAsync();
        }

        public bool IsQuizTaken(string studentId, string quizId)
        {
            return _context.StudentQuizzes.Any(sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

    }
}
