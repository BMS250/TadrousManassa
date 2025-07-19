using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IQuizRepository
    {
        public Task<Quiz?> GetQuizByIdAsync(string id);
        public Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId);
        public Task CreateQuizAsync(Quiz quiz);
        public Task UpdateQuizAsync(Quiz quiz);
        public Task DeleteQuizAsync(string id);
    }
}
