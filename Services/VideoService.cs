using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
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
        private readonly ILectureRepository _lectureRepository;
        public VideoService(IVideoRepository videoRepository, ILectureRepository lectureRepository)
        {
            _videoRepository = videoRepository;
            _lectureRepository = lectureRepository;
        }

        public Task<string?> GetQuizIdByVideoIdAsync(string videoId)
        {
            return _videoRepository.GetQuizIdByVideoIdAsync(videoId);
        }

        public async Task<OperationResult<VideoDetailsDTO>> GetVideoDetails(string lectureId, int order)
        {
            if (string.IsNullOrWhiteSpace(lectureId))
                return OperationResult<VideoDetailsDTO>.Fail("Lecture ID cannot be null or empty.");
            if (order < 1)
                return OperationResult<VideoDetailsDTO>.Fail("Order must be a positive integer.");

            var unit = await _lectureRepository.GetUnit(lectureId);
            var videoId = await _videoRepository.GetVideoIdByLectureIdAndOrder(lectureId, order);
            if (videoId == null)
                return OperationResult<VideoDetailsDTO>.Fail("Video not found.");

            VideoDetailsDTO? videoDetails = await _videoRepository.GetVideoDetailsAsync(videoId, unit);
            if (videoDetails == null)
                return OperationResult<VideoDetailsDTO>.Fail("Video not found.");

            return OperationResult<VideoDetailsDTO>.Ok(videoDetails, "Video details retrieved successfully.");
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

        public async Task<OperationResult<List<BasicDTO>>> GetVideosBasicDataByLectureIdAsync(string lectureId)
        {
            var videos = await _videoRepository.GetVideosBasicDataByLectureIdAsync(lectureId);
            if (videos is null || videos.Count == 0)
            {
                return OperationResult<List<BasicDTO>>.Fail("No videos found for the specified lecture.");
            }
            return OperationResult<List<BasicDTO>>.Ok(videos, "Videos retrieved successfully.");
        }
    }
}
