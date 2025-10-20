using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TadrousManassa.Models;
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
    }
}
