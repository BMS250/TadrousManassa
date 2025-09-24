using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class LectureController : Controller
    {
        private readonly ILectureService _lectureService;
        private readonly IStudentLectureService _studentLectureService;

        public LectureController(ILectureService lectureService, IStudentLectureService studentLectureService)
        {
            _lectureService = lectureService;
            _studentLectureService = studentLectureService;
        }

        [HttpGet]
        public IActionResult LoadLecturesTab()
        {
            var lectures = _lectureService.GetLecturesViewsCount();
            var noWatchers = _studentLectureService.GetNoWatchers();
            var viewsCountForStudents = _studentLectureService.GetViewsCountForStudents();
            LectureAnalysingPartialVM lectureWatchingVM = new()
            {
                Lectures = lectures,
                NoWatchers = noWatchers,
                ViewsCountForStudents = viewsCountForStudents
            };
            return PartialView("~/Areas/Teacher/Views/Home/_LecturePartial.cshtml", lectureWatchingVM);
        }
    }
}
