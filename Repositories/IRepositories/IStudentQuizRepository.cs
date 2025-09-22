using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentQuizRepository
    {
        public Task<string?> GetStudentQuizId(string studentId, string quizId);
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task SaveSubmissionAsync(string studentId, string quizId, Dictionary<string, string> answers);
        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public Task<bool> IsQuizSolved(string studentId, string quizId);
        //public Task<OperationResult<bool>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId);
        public Task<StudentQuiz?> GetStudentQuizAsync(string studentId, string quizId, bool includeQuiz = false);
        public Task<StudentQuizScoresDTO?> GetStudentQuizScoresAsync(string studentId, string quizId);
        public Task<float> GetBestScoreAsync(string studentId, string quizId);
        public Task AddStudentQuizAsync(StudentQuiz studentQuiz);
        public Task SaveChangesAsync();
    }
}
