using Amazon.Runtime.Internal.Util;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILectureService _lectureService;
        private readonly IStudentLectureService _studentLectureService;
        private readonly IStudentService _studentService;
        private readonly IStudentQuizService _studentQuizService;
        private readonly IQuizService _quizService;
        private readonly IVideoService _videoService;
        private readonly IStudentChoiceService _studentChoiceService;
        private readonly IMemoryCache _cache;
        private Task<ApplicationUser?> currentUser;

        public HomeController(
            ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            ILectureService lectureService,
            IStudentLectureService studentLectureService,
            IStudentService studentService,
            IStudentQuizService studentQuizService,
            IQuizService quizService,
            IVideoService videoService,
            IStudentChoiceService studentChoiceService,
            IMemoryCache cache)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _lectureService = lectureService;
            _studentLectureService = studentLectureService;
            _studentService = studentService;
            _studentQuizService = studentQuizService;
            _quizService = quizService;
            _videoService = videoService;
            _studentChoiceService = studentChoiceService;
            _cache = cache;
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
                    return View("ErrorView", "Data are incorrect");
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return View("ErrorView", "You must login first");
                }

                var buyResult = _studentLectureService.BuyCode(currentUser.Id, lectureId, code);
                if (buyResult.Success)
                {
                    TempData["success"] = "The Lecture has been purchased successfully";
                }
                else
                {
                    return View("ErrorView", buyResult.Message);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LecturePurchasing action");
                TempData["error"] = "Error in LecturePurchasing action.";
                return View("ErrorView", TempData["error"]);
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
                    return View("ErrorView", TempData["error"]); ;
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
                    return View("ErrorView", TempData["error"]); ;
                }

                var videoDetailsDTO = _lectureService.GetVideoDetails(lectureId, order);
                if (!videoDetailsDTO.Success)
                {
                    TempData["error"] = "The lecture is not found";
                    return View("ErrorView", TempData["error"]); ;
                }

                return View("lectureDetails", videoDetailsDTO.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LectureDetails action");
                TempData["error"] = "Error in LectureDetails action";
                return View("ErrorView", TempData["error"]); ;
            }
        }

        internal IActionResult DownloadSheet(string path)
        {
            // Ensure it's a valid URL to prevent open redirects
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
                return BadRequest("Invalid path");

            return Redirect(path);
        }

        [HttpGet]
        public async Task<IActionResult> QuizDetails(string? vId, string? qId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vId) && string.IsNullOrWhiteSpace(qId))
                {
                    TempData["error"] = "Video ID or Quiz ID is required.";
                    return View("ErrorView", TempData["error"]); ;
                }

                string? quizId = qId ?? await _videoService.GetQuizIdByVideoIdAsync(vId);
                if (string.IsNullOrWhiteSpace(quizId))
                {
                    TempData["error"] = "Quiz not found for the provided video.";
                    return View("ErrorView", TempData["error"]); ;
                }

                // check if the student has bought the lecture
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(quizId!);
                if (!lectureIdResult.Success || string.IsNullOrWhiteSpace(lectureIdResult.Data))
                {
                    TempData["error"] = "Lecture not found for this quiz.";
                    return View("ErrorView", TempData["error"]); ;
                }
                string lectureId = lectureIdResult.Data;

                // Get current user ID
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                var isPurchasedResult = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureId);

                if (!isPurchasedResult.Success || !isPurchasedResult.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return View("ErrorView", TempData["error"]);
                }

                var result = _studentQuizService.GetRemainingAttemptsByQuizIdAsync(currentUser.Id, quizId).Result;
                if (!result.Success)
                {
                    return View("ErrorView", result.Message);
                }
                int remainingAttempts = result.Data;

                if (remainingAttempts == 2)
                {
                    // Display the quiz details
                    var quizDetails = await _quizService.GetQuizDetailsAsync(quizId);
                    quizDetails.NumOfRemainingAttempts = remainingAttempts;
                    return View(quizDetails);
                }

                // Display the quiz result with or without score
                return RedirectToAction(nameof(QuizResult), new { qId = quizId, remainingAttempts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                TempData["error"] = "Error in SolveQuiz action";
                return View("ErrorView", TempData["error"]); ;
            }
        }

        public async Task<IActionResult> SolveQuiz(string quizId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quizId))
                    return View("ErrorView", "Quiz ID is required.");

                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return Json(new { success = false, message = "User not found" });

                // Check lecture purchase
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(quizId);
                if (!lectureIdResult.Success)
                    return View("ErrorView", lectureIdResult.Message);

                var isPurchased = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureIdResult.Data);
                if (!isPurchased.Success || !isPurchased.Data)
                    return View("ErrorView", "You must buy the lecture first.");

                // Get quiz
                var quiz = await _quizService.GetQuizByIdAsync(quizId);
                if (quiz == null)
                    return View("ErrorView", "Quiz not found.");

                // Load or create quiz start time
                //var quizStartTime = await _studentQuizService.GetOrCreateQuizStartTimeAsync(currentUser.Id, quiz.Id);

                // save Start time in cache for 2 hours
                var cacheKey = $"QuizStartTime_{currentUser.Id}_{quiz.Id}";
                if (!_cache.TryGetValue(cacheKey, out DateTime quizStartTime))
                {
                    quizStartTime = DateTime.Now;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromHours(2));
                    _cache.Set(cacheKey, quizStartTime, cacheEntryOptions);
                }

                // Build ViewModel
                var vm = new SolveQuizVM
                {
                    QuizId = quiz.Id,
                    QuizName = quiz.Name,
                    TimeHours = quiz.TimeHours,
                    TimeMinutes = quiz.TimeMinutes,
                    QuizStartTime = DateTime.Now,
                    Questions = [.. quiz.Questions.Select(q => new QuestionVM
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Image = q.Image,
                        Score = q.Score,
                        Choices = [.. q.Choices.Select(c => new ChoiceVM
                        {
                            Id = c.Id,
                            Text = c.Text,
                            Image = c.Image
                        })]
                    })]
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                return View("ErrorView", "Error in SolveQuiz action");
            }
        }

        [HttpPost]
        [ActionName("SolveQuiz")]
        public async Task<IActionResult> SubmitQuiz([FromForm] string quizId)
        {
            try
            {
                if (string.IsNullOrEmpty(quizId))
                    return Json(new { success = false, message = "Quiz Id doesn't exist" });

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return Json(new { success = false, message = "User not found" });
                var studentId = currentUser.Id;

                // Collect answers
                var answers = new Dictionary<string, string>();
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("answers[") && key.EndsWith("]"))
                    {
                        var questionId = key.Substring(8, key.Length - 9);
                        var choiceId = Request.Form[key].ToString();
                        if (!string.IsNullOrEmpty(choiceId))
                            answers[questionId] = choiceId;
                    }
                }

                _logger.LogInformation("Quiz submission received {AnswerCount} answers for quiz {QuizId} from user {UserId}",
                    answers.Count, quizId, studentId);

                var cacheKey = $"QuizStartTime_{studentId}_{quizId}";

                if (_cache.TryGetValue(cacheKey, out DateTime quizStartTime))
                {
                    _logger.LogInformation("Quiz started at: {quizStartTime}", quizStartTime);
                }
                else
                {
                    _logger.LogWarning("Quiz start time not found in cache.");
                    quizStartTime = DateTime.Now;
                }
                _cache.Remove(cacheKey);

                await _studentChoiceService.AddStudentChoicesAsync(studentId, quizId, [.. answers.Values]);

                var remainingAttemptsResult = await _studentQuizService.DecreaseNumOfRemainingAttemptsAsync(studentId, quizId);
                if (!remainingAttemptsResult.Success)
                {
                    return Json(new { success = false, message = remainingAttemptsResult.Message });
                }
                int remainingAttempts = remainingAttemptsResult.Data;

                var submissionResult = await _studentQuizService.SaveSubmissionAsync(studentId, quizId, quizStartTime, answers);
                if (!submissionResult.Success)
                {
                    return Json(new { success = false, message = submissionResult.Message });
                }
                float currentScore = submissionResult.Data;

                // Return data for POST form submission
                return Json(new
                {
                    success = true,
                    postData = new
                    {
                        quizId,
                        currentScore,
                        remainingAttempts,
                        answers
                    }
                });
            }
            catch (Exception ex)
            {
                string message = "Error processing quiz submission";
                _logger.LogError(ex, message);
                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> QuizResult([FromForm] QuizResultRequest request)
        {
            try
            {
                // Now you can access the answers dictionary
                var answers = request.Answers;

                // Check if the student has bought the lecture
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(request.QuizId);
                if (!lectureIdResult.Success || string.IsNullOrWhiteSpace(lectureIdResult.Data))
                {
                    TempData["error"] = "Lecture not found for this quiz.";
                    return View("ErrorView", TempData["error"]);
                }
                string lectureId = lectureIdResult.Data;

                // Get current user ID
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                var isPurchasedResult = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureId);

                if (!isPurchasedResult.Success || !isPurchasedResult.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return View("ErrorView", TempData["error"]);
                }

                var quizResult = await _quizService.GetQuizResultAsync(currentUser.Id, request.QuizId, request.RemainingAttempts ?? 0, answers);
                if (quizResult == null)
                {
                    TempData["error"] = $"Failed to get quiz result for user {currentUser.Id} and quiz {request.QuizId}";
                    _logger.LogError(TempData["error"]!.ToString());
                    return View("ErrorView", TempData["error"]);
                }

                quizResult.CurrentScore = request.CurrentScore;
                return View(quizResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying quiz results");
                return View();
            }
        }
    }
}