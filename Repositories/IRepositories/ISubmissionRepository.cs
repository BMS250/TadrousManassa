using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface ISubmissionRepository
    {
        public Task AddSubmissionAsync(Submission quizSubmission);
    }
}
