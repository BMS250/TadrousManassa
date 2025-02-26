﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Models;
using TadrousManassa.Services;

namespace TadrousManassa.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly ILectureService _lectureService;

        public HomeController(UserManager<ApplicationUser> userManager, IStudentService studentService, ILectureService lectureService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _lectureService = lectureService;
        }
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            var lecturesResult = _lectureService.GetLecturesVMWithUnits(userId);
            if (lecturesResult.Success)
            {
                return View(lecturesResult.Data);
            }
            return View();
        }
    }
}
