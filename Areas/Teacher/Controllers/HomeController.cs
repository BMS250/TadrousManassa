using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Text;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly IAppSettingsRepository _appSettingsRepo;
        private readonly ILectureService _lectureService;
        private readonly IStudentService _studentService;
        private readonly IStudentLectureService _studentLectureService;
        private readonly ICodeService _codeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IConfiguration configuration, 
                                ILogger<HomeController> logger, IAppSettingsRepository appSettingsRepo, 
                                ILectureService lectureService, IStudentService studentService,
                                IStudentLectureService studentLectureService, ICodeService codeService,
                                UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _appSettingsRepo = appSettingsRepo;
            _lectureService = lectureService;
            _studentService = studentService;
            _studentLectureService = studentLectureService;
            _codeService = codeService;
            _userManager = userManager;
            _codeService = codeService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult LoadSettingsTab()
        {
            var currentSettings = _appSettingsRepo.GetCurrentData();
            return PartialView("_SettingsPartial", currentSettings);
        }

        [HttpPost]
        public IActionResult UpdateSettings(DateChangingPartialVM dateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(dateVM); // Return the view with validation errors
            }
            var oldSettings = _appSettingsRepo.GetCurrentData();
            // Retrieve existing settings from the database (assuming a singleton settings entry)
            var result = _appSettingsRepo.UpdateCurrentData(dateVM.CurrentYear ?? oldSettings.CurrentYear, dateVM.CurrentSemester ?? oldSettings.CurrentSemester);
            if (!result.Success)
            {
                TempData["error"] = result.Message;
                return View(dateVM); // Return the view with an error message
            }
            TempData["success"] = "Settings updated successfully.";
            return RedirectToAction("Index"); // Redirect to settings overview page
        }

        [HttpGet]
        public IActionResult LoadVideoTab()
        {
            return PartialView("_VideoPartial");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadVideo(VideoUploadingPartialVM videoUploadingVM)
        {
            if (videoUploadingVM.Video == null || videoUploadingVM.Video.Length == 0)
            {
                TempData["error"] = "No video file provided.";
                return RedirectToAction("Index");
            }

            ApplicationSettings appSettingsData = _appSettingsRepo.GetCurrentData();
            string videoName = Path.GetFileName(videoUploadingVM.Video.FileName);
            // Create a unique object key (was) using a GUID and the original file name. /*{Guid.NewGuid()}_*/
            string videoPath = $"{videoUploadingVM.Grade ?? 1}/{appSettingsData.CurrentYear}/{appSettingsData.CurrentSemester}/{videoName}";

            try
            {
                using (var stream = videoUploadingVM.Video.OpenReadStream())
                {
                    // Reset the stream position if needed.
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }

                TempData["success"] = "Video uploaded successfully!";
                // You might choose to save the object key or URL in your database.
                Lecture lecture = new Lecture()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = videoUploadingVM.Name ?? videoName,
                    Description = videoUploadingVM.Description,
                    Grade = videoUploadingVM.Grade ?? 1,
                    Unit = videoUploadingVM.Unit,
                    Price = videoUploadingVM.Price ?? 0,
                    // TODO Add videos initialization with quizzes here and in frontend part
                    Videos = [],
                    //SheetPath = videoUploadingVM.SheetPath,
                    Semester = appSettingsData.CurrentSemester,
                    Year = appSettingsData.CurrentYear,
                    UsedThisYear = true
                };
                var result = _lectureService.AddLecture(lecture);
                if (!result.Success)
                {
                    TempData["error"] = result.Message;
                }
                else
                {
                    TempData["success"] = "Video uploaded successfully!";
                }
                // Create Codes for the lecture
                var lectureCodes = _codeService.GenerateCodes(100, result.Data.Id);
                if (lectureCodes == null)
                {
                    TempData["error"] = "Error while generating lecture codes";
                }
                else
                {
                    TempData["success"] = "Codes generated successfully!";
                    return DownloadCodes(lecture.Name, lectureCodes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error: {Message}", ex.Message);
                TempData["error"] = "An unexpected error occurred.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult DownloadCodes(string lectureName, HashSet<string> lectureCodes)
        {
            int lastDotIndex = lectureName.LastIndexOf('.');
            if (lastDotIndex > 0) // Ensure there's at least one character before the dot
            {
                lectureName = lectureName[..lastDotIndex];
            }

            var csvBuilder = new StringBuilder();
            
            foreach (var item in lectureCodes)
            {
                csvBuilder.AppendLine(item);
            }
            var preamble = Encoding.UTF8.GetPreamble();
            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var combinedBytes = preamble.Concat(csvBytes).ToArray();
            var csvStream = new MemoryStream(combinedBytes);

            return File(csvStream, "text/plain; charset=utf-8", $"{lectureName} Codes.txt");
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
            return PartialView("_LecturesPartial", lectureWatchingVM);
        }

        [HttpPost]
        public IActionResult MarkCodeAsSold([FromBody] SoldCodeDTO request)
        {
            if (string.IsNullOrEmpty(request.LectureId) || string.IsNullOrEmpty(request.Code))
                return Json(new { success = false, message = "Invalid data" });
            var result = _studentLectureService.MarkCodeAsSold(request.LectureId, request.Code);
            if (result.Success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = result.Message });
        }

        [HttpGet]
        public IActionResult LoadCodesTab()
        {
            var lectures = _lectureService.GetLecturesBasicData();
            CodeGeneratingPartialVM codeVM = new() { Lectures = lectures };
            return PartialView("_CodesPartial", codeVM);
        }

        [HttpPost]
        public IActionResult GetCode([FromQuery] string lectureId)
        {
            if (string.IsNullOrEmpty(lectureId))
            {
                return BadRequest("Lecture ID is required");
            }

            var result = _codeService.GetCode(lectureId);
            return result.Success
                ? Ok(new { code = result.Data })
                : BadRequest(result.Message);
        }

        [HttpGet]
        public IActionResult LoadResetTab()
        {
            return PartialView("_ResetDevicePartial");
        }

        [HttpPost]
        public async Task<IActionResult> ResetDeviceId(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Email is required.";
                return RedirectToAction("Index");
            }

            try
            {
                var result = await _studentService.ResetDeviceId(userEmail);
                if (result.Success)
                {
                    TempData["Success"] = $"Device ID for user {userEmail} has been reset successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to reset device ID.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
