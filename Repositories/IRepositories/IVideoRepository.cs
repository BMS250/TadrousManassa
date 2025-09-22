using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IVideoRepository
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<Video?> GetVideoByQuizIdAsync(string quizId);
        public Task<bool> IsNextVideoExistsAsync(Video currentVideo);
    }
}
