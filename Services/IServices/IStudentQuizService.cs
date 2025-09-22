using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IStudentQuizService
    {
        public Task<OperationResult<List<Quiz>>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task<OperationResult<int>> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<OperationResult<int>> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public Task<OperationResult<bool>> IsQuizSolved(string studentId, string videoId);
        public Task<OperationResult<int>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId);
        public Task<OperationResult<SavingSubmissionDTO>> SaveSubmissionAsync(string studentId, string quizId, DateTime quizStartTime, Dictionary<string, string> answers);
        public Task<OperationResult<QuizResultDTO?>> GetQuizResultOfLastSubmissionAsync(string studentId, string quizId, int remainingAttempts);
    }
}
