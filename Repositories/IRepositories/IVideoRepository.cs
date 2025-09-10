using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IVideoRepository
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<OperationResult<int?>> GetNextVideoOrderByQuizIdAsync(string lectureId, string quizId);
    }
}
