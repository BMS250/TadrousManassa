using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Models;
using TadrousManassa.Services;

namespace TadrousManassa.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly ILectureService _lectureService;

        public HomeController(UserManager<ApplicationUser> userManager, IStudentService studentService, ILectureService lectureService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _lectureService = lectureService;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            var lecturesResult = _lectureService.GetLecturesVMWithUnits(userId);
            if (lecturesResult.Success)
            {
                return View(lecturesResult.Data);
            }
            return View();
        }

        public IActionResult LectureDetails(string lectureId)
        {
            return View();
        }

        public IActionResult LecturePurchasing(string lectureId, string code)
        {
            string userId = _userManager.GetUserId(User);
            if (_lectureService.BuyCode(userId, code, lectureId).Success)
            {
                return RedirectToAction("LectureDetails", new { lectureId = lectureId });
            }
            // Replace it with an alert or anything like that
            return RedirectToAction("Index");
        }
    }
}
