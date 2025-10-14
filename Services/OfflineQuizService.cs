using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;

namespace TadrousManassa.Services.IServices
{
    public class OfflineQuizService : IOfflineQuizService
    {
        public readonly IOfflineQuizRepository _offlineQuizRepository;
        public OfflineQuizService(IOfflineQuizRepository offlineQuizRepository)
        {
            _offlineQuizRepository = offlineQuizRepository;
        }
        public async Task<OperationResult<bool>> AddOfflineQuiz(OfflineQuizDTO offlineQuizDTO)
        {
            try
            {
                OfflineQuiz offlineQuiz = new OfflineQuiz
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = offlineQuizDTO.StudentId,
                    Score = offlineQuizDTO.Score,
                    TotalScore = offlineQuizDTO.TotalScore,
                    WeekNumber = offlineQuizDTO.WeekNumber
                };
                await _offlineQuizRepository.AddOfflineQuiz(offlineQuiz);
                await _offlineQuizRepository.SaveChanges();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Fail("Failed to add offline quiz.");
            }
            
        }
    }
}
