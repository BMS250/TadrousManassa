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

        public Task<Video?> GetVideoByQuizIdAsync(string quizId)
        {
            return _context.Videos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.QuizId == quizId);
        }

        public Task<bool> IsNextVideoExistsAsync(Video currentVideo)
        {
            return _context.Videos
                .AsNoTracking()
                .AnyAsync(v => v.LectureId == currentVideo.LectureId && v.Order == currentVideo.Order + 1);
        }
    }
}
