using Amazon.Runtime.Internal.Util;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;

        public ProfileController(
            ILogger<ProfileController> logger,
            UserManager<ApplicationUser> userManager,
            IStudentService studentService)
        {
            _logger = logger;
            _userManager = userManager;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
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

                var rankResult = await _studentService.GetStudentRank(currentUser.Id);
                int rank;
                if (rankResult.Success)
                {
                    rank = rankResult.Data;
                }
                else
                {
                    rank = 0;
                }

                var profile = new Profile
                {
                    Name = currentUser.UserName ?? "",
                    Email = currentUser.Email ?? "",
                    PhoneNumber = currentUser.PhoneNumber,
                    Image = student.Data?.ProfileImage,
                    TotalScore = student.Data?.TotalScore ?? 0,
                    Rank = rank
                };

                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading profile";
                return View("ErrorView", TempData["error"]);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return Json(new { success = false, message = "No image selected" });
                }

                // Check file type
                if (!image.ContentType.StartsWith("image/"))
                {
                    return Json(new { success = false, message = "File must be an image" });
                }

                // Check file size (5MB max)
                if (image.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Image size must be less than 5MB" });
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Convert image to byte array
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Update profile image in database
                _logger.LogInformation("Updating profile image for user {UserId}", currentUser.Id);
                var updateResult = _studentService.UpdateProfileImage(currentUser.Id, imageBytes);
                if (!updateResult.Success)
                {
                    return Json(new { success = false, message = updateResult.Message });
                }

                return Json(new { 
                    success = true, 
                    message = "Image uploaded successfully",
                    imageData = Convert.ToBase64String(imageBytes)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                return Json(new { success = false, message = "Error uploading image1" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string name, string email, string phoneNumber)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Update user data
                currentUser.UserName = name;
                currentUser.Email = email;
                currentUser.PhoneNumber = phoneNumber;

                var result = await _userManager.UpdateAsync(currentUser);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = errors });
                }

                return Json(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return Json(new { success = false, message = "Error updating profile" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Check current password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(currentUser, currentPassword);
                if (!isCurrentPasswordValid)
                {
                    return Json(new { success = false, message = "Current password is incorrect" });
                }

                // Change password
                var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = errors });
                }

                return Json(new { success = true, message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return Json(new { success = false, message = "Error changing password" });
            }
        }
    }
}