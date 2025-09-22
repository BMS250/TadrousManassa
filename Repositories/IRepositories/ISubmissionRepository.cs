using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface ISubmissionRepository
    {
        public Task AddSubmissionAsync(Submission quizSubmission);
        public Task<int?> GetMaxSubmissionOrder(string studentQuizId);
        public Task<string?> GetIdOfLastSubmissionOrder(string studentQuizId, int maxOrder);
    }
}
