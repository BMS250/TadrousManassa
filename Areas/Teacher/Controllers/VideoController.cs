using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class VideoController : Controller
    {
        private readonly ILogger<VideoController> _logger;
        private readonly IAppSettingsRepository _appSettingsRepo;
        private readonly ILectureService _lectureService;
        private readonly ICodeService _codeService;

        public VideoController(ILogger<VideoController> logger, IAppSettingsRepository appSettingsRepo, 
                                ILectureService lectureService, ICodeService codeService)
        {
            _logger = logger;
            _appSettingsRepo = appSettingsRepo;
            _lectureService = lectureService;
            _codeService = codeService;
            _codeService = codeService;
        }

        [HttpGet]
        public IActionResult LoadVideoTab()
        {
            return PartialView("~/Areas/Teacher/Views/Home/_VideoPartial.cshtml");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult UploadVideo(VideoUploadingPartialVM videoUploadingVM)
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
    }
}
