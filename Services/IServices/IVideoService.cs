using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IVideoService
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<OperationResult<VideoDetailsDTO>> GetVideoDetails(string lectureId, int order);
        public Task<OperationResult<int?>> CheckAndGetNextVideoOrderByQuizIdAsync(string quizId);
    }
}
