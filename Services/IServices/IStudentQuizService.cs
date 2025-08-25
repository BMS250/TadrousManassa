using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IStudentQuizService
    {
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public bool IsQuizSolved(string studentId, string videoId);
    }
}
