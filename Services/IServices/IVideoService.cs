using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IVideoService
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<OperationResult<int?>> GetNextVideoOrderByQuizIdAsync(string lectureId, string quizId);
    }
}
