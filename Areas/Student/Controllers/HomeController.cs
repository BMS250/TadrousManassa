using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;
using TadrousManassa.Services;

namespace TadrousManassa.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class HomeController : Controller
    {
        private readonly ILectureService _lectureService;
        private readonly IStudentLectureService _studentLectureService;
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(
            ILectureService lectureService,
            IStudentLectureService studentLectureService,
            IStudentService studentService,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _lectureService = lectureService;
            _studentLectureService = studentLectureService;
            _studentService = studentService;
            _userManager = userManager;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
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

                return View(lecturesVM.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                TempData["error"] = "Error in Index action";
                return View(new LecturesBySemesterVM { LecturesOfSemestersByUnits = new Dictionary<int, List<LectureVM>>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LecturePurchasing(string lectureId, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lectureId) || string.IsNullOrWhiteSpace(code))
                {
                    TempData["error"] = "Data are incorrect";
                    return RedirectToAction(nameof(Index));
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["error"] = "You must login first.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var buyResult = _studentLectureService.BuyCode(currentUser.Id, lectureId, code);
                if (buyResult.Success)
                {
                    TempData["success"] = "The Lecture has been purchased successfully";
                }
                else
                {
                    TempData["error"] = buyResult.Message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LecturePurchasing action");
                TempData["error"] = "Error in LecturePurchasing action.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> LectureDetails(string lectureId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lectureId))
                {
                    TempData["error"] = "You must login first.";
                    return RedirectToAction(nameof(Index));
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["error"] = "You must login first.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var isPurchased = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureId);
                if (!isPurchased.Success || !isPurchased.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return RedirectToAction(nameof(Index));
                }

                var lecture = _lectureService.GetLecture(lectureId);
                if (!lecture.Success)
                {
                    TempData["error"] = "The lecture is not found";
                    return RedirectToAction(nameof(Index));
                }
                // TODO make the index by order
                TempData["VideoPath"] = lecture.Data.Videos?[0] ?? new Video();

                return View("lectureDetails", lecture.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LectureDetails action");
                TempData["error"] = "Error in LectureDetails action";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult DownloadSheet(string path)
        {
            // Ensure it's a valid URL to prevent open redirects
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
                return BadRequest("Invalid path");

            return Redirect(path);
        }

        public IActionResult TakeQuiz(string lectureId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lectureId))
                {
                    TempData["error"] = "Lecture ID is required.";
                    return RedirectToAction(nameof(Index));
                }
                //var x = _context.StudentQuizzes
                //    .Select(sq => new { sq.StudentId, sq.QuizId })
                //    .ToHashSet();
                //if (x.TryGetValue((lecture.Id, lecture.Id), out var _))
                //{
                //    return OperationResult<bool>.Fail("Cannot delete lecture because it is associated with student quizzes.");
                //}

                return View("TakeQuiz", lectureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TakeQuiz action");
                TempData["error"] = "Error in TakeQuiz action";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 