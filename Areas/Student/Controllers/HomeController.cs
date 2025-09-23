using Amazon.Runtime.Internal.Util;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILectureService _lectureService;
        private readonly IStudentService _studentService;
        private readonly IStudentQuizService _studentQuizService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ILectureService lectureService,
            IStudentService studentService,
            IStudentQuizService studentQuizService)
        {
            _logger = logger;
            _userManager = userManager;
            _lectureService = lectureService;
            _studentService = studentService;
            _studentQuizService = studentQuizService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found");
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var student = _studentService.GetStudent(currentUser.Id);
                if (!student.Success)
                {
                    _logger.LogError("Student not found for user {UserId}", currentUser.Id);
                    TempData["error"] = "Student profile not found.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var lecturesVM = _lectureService.GetLecturesVM(currentUser.Id);
                if (!lecturesVM.Success)
                {
                    _logger.LogError("Failed to get lectures for student {StudentId}", currentUser.Id);
                    TempData["error"] = lecturesVM.Message;
                    return View(new LecturesBySemesterVM { LecturesOfSemestersByUnits = new Dictionary<int, List<LectureVM>>() });
                }
                var topScores = await _studentQuizService.GetTopStudentsScoresAsync(currentUser.Id);
                lecturesVM.Data!.TopStudentsScores = topScores;
                return View(lecturesVM.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                TempData["error"] = "Error in Index action";
                return View(new LecturesBySemesterVM { LecturesOfSemestersByUnits = new Dictionary<int, List<LectureVM>>() });
            }
        }
    }
}