using Amazon.Runtime.Internal.Util;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

        public IActionResult DownloadSheet(string path)
        {
            // Ensure it's a valid URL to prevent open redirects
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
                return BadRequest("Invalid path");

            return Redirect(path);
        }

        [HttpGet]
        public async Task<IActionResult> QuizDetails(string? videoId, string? qId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(videoId) && string.IsNullOrWhiteSpace(qId))
                {
                    TempData["error"] = "Video ID or Quiz ID is required.";
                    return View("ErrorView", TempData["error"]);;
                }

                string? quizId = qId ?? await _quizService.GetQuizIdByVideoIdAsync(videoId);

                // check if the student has bought the lecture
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(quizId!);
                if (!lectureIdResult.Success || string.IsNullOrWhiteSpace(lectureIdResult.Data))
                {
                    TempData["error"] = "Lecture not found for this quiz.";
                    return View("ErrorView", TempData["error"]); ;
                }

                // Get current user ID
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                var isPurchasedResult = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureIdResult.Data);

                if (!isPurchasedResult.Success || !isPurchasedResult.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return View("ErrorView", TempData["error"]); ;
                }

                // TODO check if there is other video, if yes go to it, otherwise display finishing the lecture message

                int remainingAttempts = _studentQuizService.GetRemainingAttemptsByQuizIdAsync(currentUser.Id, quizId!).Result;

                if (remainingAttempts == 2)
                {
                    // Display the quiz details
                    var quizDetails = await _quizService.GetQuizDetailsAsync(quizId);
                    quizDetails.NumOfRemainingAttempts = remainingAttempts;
                    return View(quizDetails);
                }

                // Display the quiz result with or without score
                return RedirectToAction(nameof(QuizResult), new { qId = quizId, remainingAttempts });
                // TODO redirect then this to the view of quiz details in the quiz result in case of remaining attempts = 1
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
                {
                    TempData["error"] = "Quiz ID is required.";
                    return View("ErrorView", TempData["error"]);;
                }

                // check if the student has bought the lecture
                var lectureIdResult = await _quizService.GetLectureIdByQuizId(quizId);
                if (!lectureIdResult.Success || string.IsNullOrWhiteSpace(lectureIdResult.Data))
                {
                    TempData["error"] = "Lecture not found for this quiz.";
                    return View("ErrorView", TempData["error"]);;
                }

                // Get current user ID
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                var isPurchasedResult = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureIdResult.Data);

                if (!isPurchasedResult.Success || !isPurchasedResult.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return View("ErrorView", TempData["error"]);;
                }

                Quiz? quiz = await _quizService.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    TempData["error"] = "Quiz not found.";
                    return View("ErrorView", TempData["error"]);;
                }

                string cacheKey = $"quiz_session_{currentUser.Id}_{quizId}";

                // Check if there's an existing quiz session in cache
                var existingStartTime = _cache.Get<DateTime?>(cacheKey);

                DateTime quizStartTime;

                if (existingStartTime.HasValue)
                {
                    // Use existing start time from cache
                    quizStartTime = existingStartTime.Value;

                    // Check if quiz time has expired
                    var totalMinutes = quiz.TimeHours * 60 + quiz.TimeMinutes;
                    var elapsedTime = DateTime.UtcNow - quizStartTime;

                    if (elapsedTime.TotalMinutes >= totalMinutes)
                    {
                        // Quiz has expired, remove from cache and show expired message
                        _cache.Remove(cacheKey);
                        TempData["error"] = "Quiz time has expired.";
                        return View("ErrorView", TempData["error"]);;
                    }
                }
                else
                {
                    // Create new quiz session in cache
                    quizStartTime = DateTime.UtcNow;
                    var totalMinutes = quiz.TimeHours * 60 + quiz.TimeMinutes;

                    // Set cache with expiration slightly longer than quiz duration
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(totalMinutes + 5),
                        Priority = CacheItemPriority.High
                    };

                    _cache.Set(cacheKey, quizStartTime, cacheOptions);
                }

                // Pass the start time to the view
                ViewBag.QuizStartTime = quizStartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                return View("SolveQuiz", quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                TempData["error"] = "Error in SolveQuiz action";
                return View("ErrorView", TempData["error"]);;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SolveQuiz()
        {
            try
            {
                var quizId = Request.Form["QuizId"].ToString();

                if (string.IsNullOrEmpty(quizId))
                {
                    _logger.LogWarning("Quiz submission attempted without QuizId");
                    return Json(new { success = false, message = "Quiz Id doesn't exist" });
                }

                // Remove quiz session from cache since it's completed
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                string cacheKey = $"quiz_session_{currentUser.Id}_{quizId}";
                _cache.Remove(cacheKey);

                // قراءة إجابات الطالب من الفورم
                var answers = new Dictionary<string, string>();
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("answers[") && key.EndsWith("]"))
                    {
                        var questionId = key.Substring(8, key.Length - 9);
                        var choiceId = Request.Form[key].ToString();

                        if (!string.IsNullOrEmpty(choiceId))
                        {
                            answers[questionId] = choiceId;
                        }
                    }
                }

                _logger.LogInformation("Quiz submission received {AnswerCount} answers for quiz {QuizId} from user {UserId}",
                    answers.Count, quizId, currentUser.Id);

                // // TODO: هنا يمكنك حفظ الإجابات في قاعدة البيانات
                // حساب المحاولات المتبقية
                int remainingAttempts = await _studentQuizService.GetRemainingAttemptsByQuizIdAsync(currentUser.Id, quizId);
                remainingAttempts--; // تقليل المحاولة الحالية

                // حفظ البيانات في TempData للاستخدام في GET request
                TempData["QuizCompleted"] = true;
                TempData["QuizId"] = quizId;
                TempData["SubmissionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                TempData["AnswersCount"] = answers.Count.ToString();
                TempData["SuccessMessage"] = "تم إرسال الاختبار بنجاح!";
                TempData["StudentAnswers"] = System.Text.Json.JsonSerializer.Serialize(answers);

                // إنشاء رابط التحويل مع المعاملات المطلوبة
                var redirectUrl = Url.Action("QuizResult", "Home", new
                {
                    qId = quizId,
                    remainingAttempts = remainingAttempts,
                    area = "Student"
                });

                _logger.LogInformation("Quiz submitted successfully for quiz {QuizId}", quizId);

                // إرجاع رابط التحويل للواجهة الأمامية
                return Json(new
                {
                    success = true,
                    message = "تم إرسال الاختبار بنجاح!",
                    redirectUrl = redirectUrl
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
                //if (remainingAttempts == 1)
                //{
                //    // TODO display the result of the first attempt
                //    // TODO suggest to the user to solve the quiz again
                //    return View(nameof(QuizDetails), );
                //}

                // التحقق من وجود بيانات الاختبار المكتمل
                var quizCompleted = TempData.Peek("QuizCompleted") as bool? ?? false;
                var errorMessage = TempData.Peek("ErrorMessage")?.ToString();
                var quizId = qId ?? TempData["QuizId"]?.ToString();

                var submissionTimeStr = TempData["SubmissionTime"]?.ToString();
                var answersCountStr = TempData["AnswersCount"]?.ToString();
                var successMessage = TempData["SuccessMessage"]?.ToString();
                var studentAnswersJson = TempData["StudentAnswers"]?.ToString();


                // إذا كان هناك خطأ، عرضه
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var errorModel = new QuizResultVM
                    {
                        QuizId = quizId ?? "Unknown",
                        Message = errorMessage,
                        //IsSuccess = false,
                        SubmissionTime = DateTime.Now
                    };

                    TempData.Clear();
                    return View(errorModel);
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

                // Get current user ID
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during quiz submission");
                    return Json(new { success = false, message = "User not found" });
                }

                var isPurchasedResult = _studentLectureService.IsLecturePurchased(currentUser.Id, lectureIdResult.Data);

                if (!isPurchasedResult.Success || !isPurchasedResult.Data)
                {
                    TempData["error"] = "You must buy the lecture first.";
                    return View("ErrorView", TempData["error"]); ;
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

                var model = new QuizResultVM
                {
                    QuizId = quizId,
                    QuizName = "Quiz " + quizId, // TODO: استرجع الاسم الحقيقي من قاعدة البيانات
                    SubmissionTime = submissionTime == default ? DateTime.Now : submissionTime,
                    CorrectAnswers = answersCount,
                    TotalQuestions = answersCount, // TODO: استرجع العدد الحقيقي من قاعدة البيانات
                    Message = successMessage ?? "تم إرسال الاختبار بنجاح!",
                    //IsSuccess = true,
                    StudentAnswers = studentAnswers
                };

                // مسح TempData بعد الاستخدام
                TempData.Clear();

                _logger.LogInformation("QuizResult displayed successfully for quiz {QuizId}", quizId);
                return View(model);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                _logger.LogError(ex, "Error displaying quiz results");

                var errorModel = new QuizResultVM
                {
                    QuizId = TempData["QuizId"]?.ToString() ?? "Unknown",
                    Message = "حدث خطأ أثناء عرض النتائج",
                    //IsSuccess = false,
                    SubmissionTime = DateTime.Now
                };

                return View(errorModel);
            }
        }
    }
}