using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Teacher")]
    public class CodeController : Controller
    {
        private readonly ILectureService _lectureService;
        private readonly IStudentLectureService _studentLectureService;
        private readonly ICodeService _codeService;

        public CodeController(ILectureService lectureService, IStudentLectureService studentLectureService,
                              ICodeService codeService)
        {
            _lectureService = lectureService;
            _studentLectureService = studentLectureService;
            _codeService = codeService;
            _codeService = codeService;
        }


        [HttpPost]
        public IActionResult MarkCodeAsSold([FromBody] SoldCodeDTO request)
        {
            if (string.IsNullOrEmpty(request.LectureId) || string.IsNullOrEmpty(request.Code))
                return Json(new { success = false, message = "Invalid data" });
            var result = _studentLectureService.MarkCodeAsSold(request.LectureId, request.Code);
            if (result.Success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = result.Message });
        }

        [HttpGet]
        public IActionResult LoadCodesTab()
        {
            var lectures = _lectureService.GetLecturesBasicData();
            CodeGeneratingPartialVM codeVM = new() { Lectures = lectures };
            return PartialView("~/Areas/Teacher/Views/Home/_CodePartial.cshtml", codeVM);
        }

        [HttpPost]
        public IActionResult GetCode([FromQuery] string lectureId)
        {
            if (string.IsNullOrEmpty(lectureId))
            {
                return BadRequest("Lecture ID is required");
            }

            var result = _codeService.GetCode(lectureId);
            return result.Success
                ? Ok(new { code = result.Data })
                : BadRequest(result.Message);
        }
    }
}
