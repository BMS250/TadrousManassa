using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentQuizRepository
    {
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public bool IsQuizSolved(string studentId, string quizId);
    }
}
