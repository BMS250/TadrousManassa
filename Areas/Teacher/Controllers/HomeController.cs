using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Text;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Services;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly IAppSettingsRepository _appSettingsRepo;
        private readonly ILectureService _lectureService;
        private readonly IStudentService _studentService;
        private readonly ICodeService _codeService;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(IAmazonS3 s3Client, IConfiguration configuration, 
                                ILogger<HomeController> logger, IAppSettingsRepository appSettingsRepo, 
                                ILectureService lectureService, IStudentService studentService,
                                ICodeService codeService, 
                                UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _appSettingsRepo = appSettingsRepo;
            _lectureService = lectureService;
            _studentService = studentService;
            _codeService = codeService;
            _userManager = userManager;
            string accessKey = _configuration["AWS:AccessKey"];
            string secretKey = _configuration["AWS:SecretKey"];
            string region = _configuration["AWS:Region"];

            var regionEndpoint = RegionEndpoint.GetBySystemName(region); // Convert string to RegionEndpoint

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                _s3Client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), regionEndpoint);
            }
            else
            {
                _s3Client = new AmazonS3Client(regionEndpoint); // Use IAM Role if credentials are not provided
            }

            _codeService = codeService;
        }

        public IActionResult Index()
        {
            var oldSettings = _appSettingsRepo.GetCurrentData();
            var userId = _userManager.GetUserId(User);
            var lectures = _lectureService.GetLectures();
            var students = _studentService.GetStudents();
            AdminVM adminVM = new AdminVM()
            {
                CurrentYear = oldSettings.CurrentYear,
                CurrentSemester = oldSettings.CurrentSemester,
                Lectures = lectures,
                NoWatchers = _lectureService.GetNoWatchers()
            };
            return View(adminVM);
        }

        [HttpPost]
        public IActionResult UpdateSettings(AdminVM adminVM)
        {
            if (!ModelState.IsValid)
            {
                return View(adminVM); // Return the view with validation errors
            }
            var oldSettings = _appSettingsRepo.GetCurrentData();
            // Retrieve existing settings from the database (assuming a singleton settings entry)
            var result = _appSettingsRepo.UpdateCurrentData(adminVM.CurrentYear ?? oldSettings.CurrentYear, adminVM.CurrentSemester ?? oldSettings.CurrentSemester);
            if (!result.Success)
            {
                TempData["error"] = result.Message;
                return View(adminVM); // Return the view with an error message
            }
            TempData["success"] = "Settings updated successfully.";
            return RedirectToAction("Index"); // Redirect to settings overview page
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(AdminVM adminVM)
        {
            if (adminVM.Video == null || adminVM.Video.Length == 0)
            {
                TempData["error"] = "No video file provided.";
                return RedirectToAction("Index");
            }

            // Load bucket name from configuration (e.g., appsettings.json under "AWS:BucketName")
            string bucketName = _configuration["AWS:BucketName"];

            ApplicationSettings appSettingsData = _appSettingsRepo.GetCurrentData();
            string videoName = Path.GetFileName(adminVM.Video.FileName);
            // Create a unique object key (was) using a GUID and the original file name. /*{Guid.NewGuid()}_*/
            string videoPath = $"{adminVM.Grade ?? 1}/{appSettingsData.CurrentYear}/{appSettingsData.CurrentSemester}/{videoName}";

            try
            {
                using (var stream = adminVM.Video.OpenReadStream())
                {
                    // Reset the stream position if needed.
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }


                    var transferUtility = new TransferUtility(_s3Client);
                    // Asynchronously upload the video stream to S3.
                    await transferUtility.UploadAsync(stream, bucketName, videoPath);
                }

                TempData["success"] = "Video uploaded successfully!";
                // You might choose to save the object key or URL in your database.
                Lecture lecture = new Lecture()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = adminVM.Name ?? videoName,
                    Description = adminVM.Description,
                    Grade = adminVM.Grade ?? 1,
                    Unit = adminVM.Unit,
                    Price = adminVM.Price ?? 0,
                    VideoPath = videoPath,
                    SheetPath = adminVM.SheetPath,
                    QuizPath = adminVM.QuizPath,
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
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex, "AWS Service Error: {Message}", ex.Message);
                TempData["error"] = "Error uploading video. Check AWS permissions.";
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

        [HttpGet("play-count/{lectureId}")]
        public IActionResult GetPlayCount(string lectureId)
        {
            try
            {
                int playCount = _lectureService.GetViewsCount(lectureId);
                return Json(new { count = playCount });
            }
            catch (Exception)
            {
                return NotFound(new { message = "Could not retrieve play count" });
            }
        }
    }
}
