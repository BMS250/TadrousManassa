using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class VideoRepository(ApplicationDbContext context) : IVideoRepository
    {
        private readonly ApplicationDbContext _context = context;


        public async Task<string?> GetQuizIdByVideoIdAsync(string videoId)
        {
            return await _context.Videos 
                .AsNoTracking()
                .Where(v => v.Id == videoId)
                .Select(v => v.QuizId)
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult<int?>> GetNextVideoOrderByQuizIdAsync(string lectureId, string quizId)
        {
            int? currentOrder = _context.Videos?
                .AsNoTracking()?
                .FirstOrDefault(v => v.QuizId == quizId)?.Order;

            if (currentOrder == null)
                return await Task.FromResult(OperationResult<int?>.Fail("Quiz not found."));

            bool hasNext = await _context.Videos
                .AsNoTracking()
                .AnyAsync(v => v.LectureId == lectureId && v.Order == currentOrder + 1);

            if (!hasNext)
            {
                return await Task.FromResult(OperationResult<int?>.Fail("This is the last video in the lecture."));
            }
            return await Task.FromResult(OperationResult<int?>.Ok(currentOrder + 1, "Next video order retrieved successfully."));
        }
    }
}
