using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    //[Authorize(Roles = "Teacher")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;

        public HomeController(UserManager<ApplicationUser> userManager, IStudentService studentService)
        {
            _userManager = userManager;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var roles = _userManager.GetRolesAsync(currentUser).Result;
            foreach (var role in roles)
            {
                Console.WriteLine(role);    
            }
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddStudent([FromBody] TadrousManassa.Models.Student student)
        {
            var result = await _studentService.InsertStudentAsync(student);
            if (result.Success)
            {
                return Ok("Student added successfully.");
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpGet]
        public IActionResult LoadAddStudentTab()
        {
            return PartialView("_StudentPartial", new RegisterStudentVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(RegisterStudentVM model)
        {
            if (!ModelState.IsValid)
                return PartialView("_StudentPartial", model);



            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber ?? "",
                Student = null
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return PartialView("_StudentPartial", model);
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new TadrousManassa.Models.Student
            {
                Id = user.Id,
                Address = model.Address ?? "",
                PhoneNumber_Parents = model.PhoneNumber_Parents ?? "",
                School = model.School ?? "",
                Grade = model.Grade,
                ReferralSource = model.ReferralSource ?? "",
                DeviceId = "000",
                TotalScore = 0,
                ApplicationUser = user
            };

            await _studentService.InsertStudentAsync(student);
            return Content("Student registered successfully.");
        }

    }
}
