using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class DeviceController : Controller
    {
        private readonly IStudentService _studentService;

        public DeviceController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public IActionResult LoadResetTab()
        {
            return PartialView("~/Areas/Teacher/Views/Home/_DevicePartial.cshtml");
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
