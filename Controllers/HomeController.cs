using Microsoft.AspNetCore.Mvc;

namespace TadrousManassa.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// يعرض صفحة خطأ بسيطة
        /// </summary>
        /// <param name="message">رسالة الخطأ (اختياري)</param>
        /// <returns>صفحة الخطأ</returns>
        [HttpGet]
        public IActionResult Error(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى.";
            }

            return View("ErrorView", message);
        }
    }
}
