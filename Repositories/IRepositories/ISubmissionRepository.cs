using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface ISubmissionRepository
    {
        public Task AddSubmissionAsync(QuizSubmission quizSubmission);
    }
}
