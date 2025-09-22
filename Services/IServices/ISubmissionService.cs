using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface ISubmissionService
    {
        public Task AddSubmissionAsync(Submission quizSubmission);
        public Task<OperationResult<int>> GetMaxSubmissionOrder(string studentId, string quizId);
        public Task<string?> GetIdOfLastSubmissionOrder(string studentQuizId, int maxOrder);
    }
}
