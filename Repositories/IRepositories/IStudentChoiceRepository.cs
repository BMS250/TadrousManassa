using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentChoiceRepository
    {
        public Task<StudentChoice?> GetStudentChoiceByIdAsync(string id);
        public Task<Dictionary<string, bool>> GetCorrectnessAsync(string studentId, string quizId);
        public Task<Dictionary<string, KeyValuePair<bool?, string>>> GetCorrectnessAndRightAnswersAsync(string studentId, string quizId);
        public Task AddStudentChoicesAsync(string studentId, string quizId, List<string> answerIds);
        public Task<int> SaveChangesAsync();
    }
}
