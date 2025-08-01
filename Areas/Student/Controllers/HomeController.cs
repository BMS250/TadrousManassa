using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> SolveQuiz(string quizId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quizId))
                {
                    TempData["error"] = "Quiz ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                Quiz? quiz = await _quizService.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    TempData["error"] = "Quiz not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View("SolveQuiz", quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SolveQuiz action");
                TempData["error"] = "Error in SolveQuiz action";
                return RedirectToAction(nameof(Index));
            }
        }

        // Controller Actions
        [HttpPost]
        public IActionResult QuizResult()
        {
            try
            {
                // قراءة معرف الاختبار
                var quizId = Request.Form["quizId"].ToString();

                if (string.IsNullOrEmpty(quizId))
                {
                    TempData["ErrorMessage"] = "Quiz Id doesn't exist";
                    return RedirectToAction("Index", "Home");
                }

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

                // TODO: هنا يمكنك حفظ الإجابات في قاعدة البيانات
                // SaveQuizAnswers(quizId, answers);

                // حفظ البيانات في TempData للاستخدام في GET request
                TempData["QuizCompleted"] = true;
                TempData["QuizId"] = quizId;
                TempData["SubmissionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                TempData["AnswersCount"] = answers.Count.ToString();
                TempData["SuccessMessage"] = "The quiz is submitted successfully!";

                // حفظ الإجابات كـ JSON في TempData (اختياري للعرض)
                TempData["StudentAnswers"] = System.Text.Json.JsonSerializer.Serialize(answers);

                return RedirectToAction("QuizResult", new { id = quizId });
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                // _logger.LogError(ex, "Error processing quiz submission");

                TempData["ErrorMessage"] = "Error while submitting the quiz, please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult QuizResult(string id)
        {
            try
            {
                // التحقق من وجود بيانات الاختبار المكتمل
                var quizCompleted = TempData.Peek("QuizCompleted") as bool? ?? false;
                var errorMessage = TempData.Peek("ErrorMessage")?.ToString();

                // إذا كان هناك خطأ، عرضه
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var errorModel = new QuizResultViewModel
                    {
                        QuizId = id ?? "Unknown",
                        Message = errorMessage,
                        IsSuccess = false,
                        SubmissionTime = DateTime.Now
                    };

                    // مسح TempData بعد الاستخدام
                    TempData.Clear();
                    return View(errorModel);
                }

                // إذا لم يتم إكمال الاختبار بشكل صحيح
                if (!quizCompleted)
                {
                    return RedirectToAction("Index", "Home");
                }

                // استرجاع البيانات من TempData
                var quizId = TempData["QuizId"]?.ToString() ?? id;
                var submissionTimeStr = TempData["SubmissionTime"]?.ToString();
                var answersCountStr = TempData["AnswersCount"]?.ToString();
                var successMessage = TempData["SuccessMessage"]?.ToString();
                var studentAnswersJson = TempData["StudentAnswers"]?.ToString();

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
                    catch
                    {
                        // في حالة فشل تحويل JSON، استخدم قاموس فارغ
                        studentAnswers = new Dictionary<string, string>();
                    }
                }

                // TODO: هنا يمكنك استرجاع معلومات إضافية من قاعدة البيانات
                // var quizInfo = GetQuizInfo(quizId);

                // إنشاء الـ ViewModel
                var model = new QuizResultViewModel
                {
                    QuizId = quizId,
                    QuizName = "Quiz " + quizId, // TODO: استرجع الاسم الحقيقي من قاعدة البيانات
                    SubmissionTime = submissionTime == default ? DateTime.Now : submissionTime,
                    AnsweredQuestions = answersCount,
                    TotalQuestions = answersCount, // TODO: استرجع العدد الحقيقي من قاعدة البيانات
                    Message = successMessage ?? "The quiz is submitted successfully!",
                    IsSuccess = true,
                    StudentAnswers = studentAnswers
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ
                // _logger.LogError(ex, "Error displaying quiz results");

                var errorModel = new QuizResultViewModel
                {
                    QuizId = id ?? "Unknown",
                    Message = "حدث خطأ أثناء عرض النتائج",
                    IsSuccess = false,
                    SubmissionTime = DateTime.Now
                };

                return View(errorModel);
            }
        }
    }
}