using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface IStudentQuizRepository
    {
        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId);
        public bool IsQuizTaken(string studentId, string quizId);
    }
}
