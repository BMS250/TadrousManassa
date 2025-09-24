using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class QuizController : Controller
    {
        private readonly ILectureService _lectureService;
        private readonly IVideoService _videoService;

        public QuizController(ILectureService lectureService, IVideoService videoService)
        {
            _lectureService = lectureService;
            _videoService = videoService;
        }

        [HttpGet]
        public IActionResult LoadQuizTab()
        {
            return PartialView("~/Areas/Teacher/Views/Home/_QuizPartial.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetLecturesByGrade(int grade)
        {
            try
            {
                // Your logic to get lectures by grade
                var lectures = await _lectureService.GetLecturesBasicDataByGrade(grade);
                if (lectures.Success)
                {
                    return Json(lectures.Data);
                }
                return Json(new { error = lectures.Message });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVideosByLecture(string lectureId)
        {
            try
            {
                // Your logic to get videos by lectureId
                var videos = await _videoService.GetVideosBasicDataByLectureIdAsync(lectureId);
                if (videos.Success)
                {
                    return Json(videos.Data);
                }
                return Json(new { error = videos.Message });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateQuiz()
        {
            return PartialView("~/Areas/Teacher/Views/Home/_QuizPartial.cshtml");
        }
    }
}
