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
                    return View("ErrorView", TempData["error"]);;
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
                    return View("ErrorView", TempData["error"]);;
                }

                var videoDetailsDTO = _lectureService.GetVideoDetails(lectureId, order);
                if (!videoDetailsDTO.Success)
                {
                    TempData["error"] = "The lecture is not found";
                    return View("ErrorView", TempData["error"]);;
                }

                return View("lectureDetails", videoDetailsDTO.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LectureDetails action");
                TempData["error"] = "Error in LectureDetails action";
                return View("ErrorView", TempData["error"]);;
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
                    return View("ErrorView", TempData["error"]);;
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

                int remainingAttempts = _studentQuizService.GetRemainingAttemptsByQuizIdAsync(currentUser.Id, quizId).Result;

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
                return View("ErrorView", TempData["error"]);;
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
                    Questions = quiz.Questions.Select(q => new QuestionVM
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Image = q.Image,
                        Score = q.Score,
                        Choices = q.Choices.Select(c => new ChoiceVM
                        {
                            Id = c.Id,
                            Text = c.Text,
                            Image = c.Image
                        }).ToList()
                    }).ToList()
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
                    answers.Count, quizId, currentUser.Id);

                var cacheKey = $"QuizStartTime_{currentUser.Id}_{quizId}";

                if (_cache.TryGetValue(cacheKey, out DateTime quizStartTime))
                {
                    // Found in cache → quizStartTime is the original DateTime
                    _logger.LogInformation("Quiz started at: {quizStartTime}", quizStartTime);
                }
                else
                {
                    // Not found → maybe expired or never set
                    _logger.LogWarning("Quiz start time not found in cache.");
                    quizStartTime = DateTime.Now; // Default to now if not found
                }
                _cache.Remove(cacheKey);
                // Save answers in DB (instead of TempData) and return remaining attempts
                int remainingAttempts = await _studentQuizService.SaveQuizSubmissionAsync(currentUser.Id, quizId, quizStartTime, answers);

                var redirectUrl = Url.Action("QuizResult", "Home", new
                {
                    qId = quizId,
                    remainingAttempts,
                    area = "Student"
                });

                return Json(new
                {
                    success = true,
                    message = "تم إرسال الاختبار بنجاح!",
                    redirectUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing quiz submission");
                return Json(new { success = false, message = "حدث خطأ أثناء إرسال الاختبار، يرجى المحاولة مرة أخرى." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuizResult(string? qId, int? remainingAttempts)
        {
            try
            {
                // التحقق من وجود بيانات الاختبار المكتمل
                var quizCompleted = TempData.Peek("QuizCompleted") as bool? ?? false;
                var errorMessage = TempData.Peek("ErrorMessage")?.ToString();
                var quizId = qId ?? TempData["QuizId"]?.ToString();

                var submissionTimeStr = TempData["SubmissionTime"]?.ToString();
                var answersCountStr = TempData["AnswersCount"]?.ToString();
                var successMessage = TempData["SuccessMessage"]?.ToString();
                var studentAnswersJson = TempData["StudentAnswers"]?.ToString();


                if (!string.IsNullOrEmpty(errorMessage))
                {
                    //var errorModel = new QuizResultVM
                    //{
                    //    QuizId = quizId ?? "Unknown",
                    //    Message = errorMessage,
                    //    //IsSuccess = false,
                    //    SubmissionTime = DateTime.Now
                    //};

                    TempData.Clear();
                    return View(/*errorModel*/);
                }

                if (string.IsNullOrWhiteSpace(quizId))
                {
                    TempData["error"] = "Quiz ID is required.";
                    return View("ErrorView", TempData["error"]); ;
                }

                // check if the student has bought the lecture
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(quizId);
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
                    return View("ErrorView", TempData["error"]); ;
                }

                // Ensure that all these conditions are mandatory (meaning that remaning attempts must be 0 or 1)
                if (string.IsNullOrWhiteSpace(studentAnswersJson) && string.IsNullOrWhiteSpace(answersCountStr) && string.IsNullOrWhiteSpace(submissionTimeStr))
                {
                    // display the user's solutions, if they are correct or not and the score

                    // TODO display the result of the first attempt
                    // TODO suggest to the user to solve the quiz again if the score is not the full mark

                    var quizResult = await _quizService.GetQuizResultAsync(currentUser.Id, quizId, remainingAttempts ?? 0);
                    if (quizResult is null)
                    {
                        TempData["error"] = $"Failed to get quiz result for user {currentUser.Id} and quiz {quizId}";
                        _logger.LogError(TempData["error"]!.ToString());
                        return View("ErrorView", TempData["error"]);
                    }
                    return View(quizResult);
                }

                // إذا لم يتم إكمال الاختبار بشكل صحيح
                if (!quizCompleted)
                {
                    _logger.LogWarning("QuizResult accessed without proper completion data");
                    return RedirectToAction("Index", "Home");
                }

                // تحويل البيانات
                DateTime.TryParse(submissionTimeStr, out var submissionTime);
                int.TryParse(answersCountStr, out var answersCount);

                var studentAnswers = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(studentAnswersJson))
                {
                    try
                    {
                        studentAnswers = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(studentAnswersJson)
                            ?? new Dictionary<string, string>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing student answers JSON");
                        // في حالة فشل تحويل JSON، استخدم قاموس فارغ
                        studentAnswers = new Dictionary<string, string>();
                    }
                }

                //var model = new QuizResultVM
                //{
                //    QuizId = quizId,
                //    QuizName = "Quiz " + quizId, // TODO: استرجع الاسم الحقيقي من قاعدة البيانات
                //    SubmissionTime = submissionTime == default ? DateTime.Now : submissionTime,
                //    CorrectAnswers = answersCount,
                //    TotalQuestions = answersCount, // TODO: استرجع العدد الحقيقي من قاعدة البيانات
                //    Message = successMessage ?? "تم إرسال الاختبار بنجاح!",
                //    //IsSuccess = true,
                //    StudentAnswers = studentAnswers
                //};

                // مسح TempData بعد الاستخدام
                TempData.Clear();
                _logger.LogInformation("QuizResult displayed successfully for quiz {QuizId}", quizId);

                await _studentQuizService.DecreaseNumOfRemainingAttemptsAsync(currentUser.Id, quizId);

                // TODO check if there is other video, if yes go to it, otherwise display finishing the lecture message
                var nextVideoOrderResult = await _videoService.GetNextVideoOrderByQuizIdAsync(lectureId, quizId);
                if (!nextVideoOrderResult.Success)
                {
                    TempData["error"] = nextVideoOrderResult.Message;
                    return View("ErrorView", TempData["error"]);
                }


                return View(/*model*/);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                _logger.LogError(ex, "Error displaying quiz results");

                //var errorModel = new QuizResultVM
                //{
                //    QuizId = TempData["QuizId"]?.ToString() ?? "Unknown",
                //    Message = "حدث خطأ أثناء عرض النتائج",
                //    //IsSuccess = false,
                //    SubmissionTime = DateTime.Now
                //};

                return View(/*errorModel*/);
            }
        }
    }
}