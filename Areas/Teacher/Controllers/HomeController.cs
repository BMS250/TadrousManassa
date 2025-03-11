using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

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

        public HomeController(IAmazonS3 s3Client, IConfiguration configuration, ILogger<HomeController> logger, IAppSettingsRepository appSettingsRepo)
        {
            _configuration = configuration;
            _logger = logger;
            _appSettingsRepo = appSettingsRepo;
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
        }

        public IActionResult Index()
        {
            var oldSettings = _appSettingsRepo.GetCurrentData();
            AdminVM adminVM = new AdminVM()
            {
                CurrentYear = oldSettings.CurrentYear,
                CurrentSemester = oldSettings.CurrentSemester
            };
            return View(adminVM);
        }

        //[HttpGet]
        //public IActionResult UpdateSettings()
        //{
        //    return View();
        //}


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
                TempData["ErrorMessage"] = result.Message;
                return View(adminVM); // Return the view with an error message
            }
            TempData["SuccessMessage"] = "Settings updated successfully.";
            return RedirectToAction("Index"); // Redirect to settings overview page
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(AdminVM adminVM)
        {
            if (adminVM.Video == null || adminVM.Video.Length == 0)
            {
                TempData["ErrorMessage"] = "No video file provided.";
                return RedirectToAction("Index");
            }

            // Load bucket name from configuration (e.g., appsettings.json under "AWS:BucketName")
            string bucketName = _configuration["AWS:BucketName"];

            ApplicationSettings appSettingsData = _appSettingsRepo.GetCurrentData();

            // Create a unique object key (was) using a GUID and the original file name. /*{Guid.NewGuid()}_*/
            string objectKey = $"{adminVM.Grade}/{appSettingsData.CurrentYear}/{appSettingsData.CurrentSemester}/{Path.GetFileName(adminVM.Video.FileName)}";

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
                    await transferUtility.UploadAsync(stream, bucketName, objectKey);
                }

                //// Optionally, generate a pre-signed URL for immediate access.
                //var preSignedRequest = new GetPreSignedUrlRequest
                //{
                //    BucketName = bucketName,
                //    Key = objectKey,
                //    Expires = DateTime.UtcNow.AddHours(1), // URL valid for 1 hour
                //    Verb = HttpVerb.GET
                //};
                //string videoUrl = _s3Client.GetPreSignedURL(preSignedRequest);

                TempData["SuccessMessage"] = "Video uploaded successfully!";
                // You might choose to save the object key or URL in your database.
            }
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex, "AWS Service Error: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Error uploading video. Check AWS permissions.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error: {Message}", ex.Message);
                TempData["ErrorMessage"] = "An unexpected error occurred.";
            }
            return RedirectToAction("Index");
        }
    }
}
