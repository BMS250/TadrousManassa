using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Utilities;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAmazonS3 s3Client, IConfiguration configuration, ILogger<HomeController> logger)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(LectureVM lectureVM)
        {
            if (lectureVM.Video == null || lectureVM.Video.Length == 0)
            {
                TempData["ErrorMessage"] = "No video file provided.";
                return RedirectToAction("Index");
            }

            // Load bucket name from configuration (e.g., appsettings.json under "AWS:BucketName")
            string bucketName = _configuration["AWS:BucketName"];

            // Create a unique object key (was) using a GUID and the original file name. /*{Guid.NewGuid()}_*/
            string objectKey = $"{lectureVM.Grade}/{ApplicationSettings.CurrentYear}/{ApplicationSettings.CurrentSemester}/{Path.GetFileName(lectureVM.Video.FileName)}";

            try
            {
                using (var stream = lectureVM.Video.OpenReadStream())
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video.");
                TempData["ErrorMessage"] = "Error uploading video. Please try again later.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UploadVideo()
        {
            return RedirectToAction("Index");
        }
    }
}
