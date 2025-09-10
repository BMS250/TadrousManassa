using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        public VideoService(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public Task<string?> GetQuizIdByVideoIdAsync(string videoId)
        {
            return _videoRepository.GetQuizIdByVideoIdAsync(videoId);
        }

        public Task<OperationResult<int?>> GetNextVideoOrderByQuizIdAsync(string lectureId, string quizId)
        {
            return _videoRepository.GetNextVideoOrderByQuizIdAsync(lectureId, quizId);
        }
    }
}
