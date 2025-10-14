using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class OfflineQuizController : Controller
    {
        private readonly ILectureService _lectureService;
        private readonly IVideoService _videoService;
        private readonly IStudentService _studentService;
        private readonly IOfflineQuizService _offlineQuizService;

        public OfflineQuizController(ILectureService lectureService, IVideoService videoService, IStudentService studentService, IOfflineQuizService offlineQuizService)
        {
            _lectureService = lectureService;
            _videoService = videoService;
            _studentService = studentService;
            _offlineQuizService = offlineQuizService;
        }

        [HttpGet]
        public IActionResult LoadOfflineQuizTab()
        {
            return PartialView("~/Areas/Teacher/Views/Home/_OfflineQuizPartial.cshtml");
        }

        [HttpGet]
        public IActionResult SearchStudents(string query, string type = "name", int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { success = true, data = new List<object>() });

            var result = _studentService.SearchStudents(query, type, limit);
            if (!result.Success)
                return Json(new { success = false, message = result.Message ?? "Search failed" });

            var data = result.Data!.Select(s => new
            {
                id = s.Id,
                name = s.ApplicationUser?.UserName,
                email = s.ApplicationUser?.Email
            }).ToList();

            return Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> AddOfflineQuiz(OfflineQuizDTO offlineQuizDTO)
        {
            if (offlineQuizDTO == null || string.IsNullOrWhiteSpace(offlineQuizDTO.StudentId))
                return Json(new { success = false, message = "Invalid offline quiz data" });
            var result = await _offlineQuizService.AddOfflineQuiz(offlineQuizDTO);
            if (!result.Success)
                return Json(new { success = false, message = result.Message ?? "Failed to add offline quiz" });
            return Json(new { success = true, message = "Offline quiz added successfully" });
        }
    }
}
