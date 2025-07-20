using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IQuizRepository
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<Quiz?> GetQuizByIdAsync(string id);
        public Task<QuizDetailsDTO?> GetQuizDetailsAsync(string id);
        //public Task<Quiz?> GetQuizByVideoIdAsync(string videoId);
        public Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId);
        public Task CreateQuizAsync(Quiz quiz);
        public Task UpdateQuizAsync(Quiz quiz);
        public Task DeleteQuizAsync(string id);
    }
}
