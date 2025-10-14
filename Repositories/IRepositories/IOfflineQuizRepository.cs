using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IOfflineQuizRepository
    {
        Task AddOfflineQuiz(OfflineQuiz offlineQuiz);
        Task SaveChanges();
    }
}
