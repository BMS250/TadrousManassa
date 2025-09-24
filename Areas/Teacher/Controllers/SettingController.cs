using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class SettingController : Controller
    {
        private readonly IAppSettingsRepository _appSettingsRepo;

        public SettingController(IAppSettingsRepository appSettingsRepo)
        {
            _appSettingsRepo = appSettingsRepo;
        }

        [HttpGet]
        public IActionResult LoadSettingsTab()
        {
            var currentSettings = _appSettingsRepo.GetCurrentData();
            return PartialView("~/Areas/Teacher/Views/Home/_SettingPartial.cshtml", currentSettings);
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
    }
}
