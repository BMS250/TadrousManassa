using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentQuizRepository
    {
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public Task<bool> IsQuizSolved(string studentId, string quizId);
        //public Task<OperationResult<bool>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId);
        public Task<StudentQuiz?> GetStudentQuizAsync(string studentId, string quizId);
        public Task SaveChangesAsync();
    }
}
