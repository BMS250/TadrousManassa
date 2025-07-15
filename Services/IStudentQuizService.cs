using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public interface IStudentQuizService
    {
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public bool IsQuizTaken(string studentId, string quizId);
    }
}
