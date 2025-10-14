using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;

namespace TadrousManassa.Services.IServices
{
    public interface IOfflineQuizService
    {
        Task<OperationResult<bool>> AddOfflineQuiz(OfflineQuizDTO offlineQuizDTO);
    }
}
