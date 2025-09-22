using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentQuizRepository
    {
        public Task<string?> GetStudentQuizId(string studentId, string quizId);
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId);
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId);
        public Task<bool> IsQuizSolved(string studentId, string quizId);
        public Task<StudentQuiz?> GetStudentQuizAsync(string studentId, string quizId, bool includeQuiz = false);
        public Task<StudentQuizScoresDTO?> GetStudentQuizScoresAsync(string studentId, string quizId);
        public Task<float> GetBestScoreAsync(string studentId, string quizId);
        public Task<List<TopStudentsScores>> GetTopStudentsScoresAsync(string studentId, int topN = 3);
        public Task AddStudentQuizAsync(StudentQuiz studentQuiz);
        public Task SaveChangesAsync();
    }
}
