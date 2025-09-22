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

        public async Task<OperationResult<int?>> CheckAndGetNextVideoOrderByQuizIdAsync(string quizId)
        {
            var currentVideo = await _videoRepository.GetVideoByQuizIdAsync(quizId);

            if (currentVideo == null)
                return OperationResult<int?>.Fail("Quiz not found.");

            bool hasNext = await _videoRepository.IsNextVideoExistsAsync(currentVideo);

            if (!hasNext)
            {
                return await Task.FromResult(OperationResult<int?>.Ok(-1, "This is the last video in the lecture."));
            }
            return await Task.FromResult(OperationResult<int?>.Ok(currentVideo.Order + 1, "Next video order retrieved successfully."));
        }
    }
}
