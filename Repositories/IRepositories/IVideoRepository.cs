using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IVideoRepository
    {
        public Task<string?> GetQuizIdByVideoIdAsync(string videoId);
        public Task<Video?> GetVideoByQuizIdAsync(string quizId);
        public Task<string?> GetVideoIdByLectureIdAndOrder(string lectureId, int order);
        public Task<string?> GetVideoPath(string id);
        public Task<VideoDetailsDTO?> GetVideoDetailsAsync(string id, string unit);
        public Task<bool> IsNextVideoExistsAsync(Video currentVideo);
        public Task<List<BasicDTO>> GetVideosBasicDataByLectureIdAsync(string lectureId);
    }
}
