using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILectureService _lectureService;
        private readonly IStudentLectureService _studentLectureService;
        private readonly IStudentService _studentService;
        private readonly IStudentQuizService _studentQuizService;
        private readonly IQuizService _quizService;

        public HomeController(
            ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            ILectureService lectureService,
            IStudentLectureService studentLectureService,
            IStudentService studentService,
            IStudentQuizService studentQuizService,
            IQuizService quizService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _lectureService = lectureService;
            _studentLectureService = studentLectureService;
            _studentService = studentService;
            _studentQuizService = studentQuizService;
            _quizService = quizService;
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

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> LectureDetails(string lectureId, int order = 0)
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

                //var lecture = _lectureService.GetLecture(lectureId);
                //if (!lecture.Success)
                //{
                //    TempData["error"] = "The lecture is not found";
                //    return RedirectToAction(nameof(Index));
                //}
                //// TODO make the index by order
                //TempData["VideoPath"] = lecture.Data.Videos?[0] ?? new Video();

                //return View("lectureDetails", lecture.Data);
                var videoDetailsDTO = _lectureService.GetVideoDetails(lectureId, order);
                if (!videoDetailsDTO.Success)
                {
                    TempData["error"] = "The lecture is not found";
                    return RedirectToAction(nameof(Index));
                }
                // TODO make the index by order
                //TempData["VideoPath"] = videoDetailsDTO.Data?.Path ?? "";

                return View("lectureDetails", videoDetailsDTO.Data);
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

        [HttpGet]
        public async Task<IActionResult> QuizDetails(string videoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(videoId))
                {
                    TempData["error"] = "Video ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if the quiz is already solved
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["error"] = "You must login first.";
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                string? quizId = await _quizService.GetQuizIdByVideoIdAsync(videoId);
                if (string.IsNullOrWhiteSpace(quizId))
                {
                    // TODO check if there is other video, if yes go to it, otherwise display finishing the lecture message
                    TempData["error"] = "Quiz not found for this video.";
                    return RedirectToAction(nameof(Index));
                }
                int remainingAttempts = _studentQuizService.GetRemainingAttemptsAsync(currentUser.Id, videoId).Result;

                if (remainingAttempts == 0)
                {
                    // TODO Display the quiz result with score
                    TempData["error"] = "You have already solved this quiz.";
                    return RedirectToAction(nameof(Index));
                }
                else if (remainingAttempts == 1)
                {
                    // TODO Display the quiz result without score
                    TempData["error"] = "You have already solved this quiz.";
                    return RedirectToAction(nameof(Index));
                }

                // Display the quiz details
                var quizDetails = await _quizService.GetQuizDetailsAsync(quizId);
                quizDetails.NumOfRemainingAttempts = remainingAttempts;
                return View("QuizDetails", quizDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                TempData["error"] = "Error in SolveQuiz action";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult SolveQuiz(string quizId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quizId))
                {
                    TempData["error"] = "Quiz ID is required.";
                    return RedirectToAction(nameof(Index));
                }
                //var x = _context.StudentQuizzes
                //    .Select(sq => new { sq.StudentId, sq.QuizId })
                //    .ToHashSet();
                //if (x.TryGetValue((lecture.Id, lecture.Id), out var _))
                //{
                //    return OperationResult<bool>.Fail("Cannot delete lecture because it is associated with student quizzes.");
                //}
                //var quiz = _quizService.GetQuizByVideoIdAsync(videoId);

                return View("SolveQuiz", quizId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                TempData["error"] = "Error in SolveQuiz action";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult QuizResult()
        {
            return View();
        }
        [HttpPost("/Student/Home/add-quiz")]
        public async Task<IActionResult> AddQuiz(string id/*[FromBody] Quiz quiz*/)
        {
            _logger.LogInformation("=== AddQuiz ACTION CALLED ===");
            _logger.LogInformation("Request Headers: {Headers}", Request.Headers.ToString());
            _logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);
            //_logger.LogInformation("Quiz object is null: {IsNull}", quiz == null);

            //if (quiz != null)
            //{
            //    _logger.LogInformation("Quiz data: {@Quiz}", quiz);
            //}
            Quiz quiz = new Quiz
            {
                Id = id,
                VideoId = "ferf",
                Description = "tyki",
                TimeHours = 0,
                TimeMinutes = 30,
                LectureId = "LectureId", // Replace with actual lecture ID
                Questions = new List<Question>() // Initialize with an empty list or add questions as needed
            };
            //try
            //{
            //    // Your existing code here...
            await _quizService.CreateQuizAsync(quiz);

            //    _logger.LogInformation("Quiz created successfully");
                return Ok(new { success = true, message = "Quiz added successfully." });
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error in AddQuiz action");
            //    return BadRequest(new { success = false, message = ex.Message });
            //}
        }


    }
} 