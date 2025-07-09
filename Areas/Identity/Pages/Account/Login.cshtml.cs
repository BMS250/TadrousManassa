// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TadrousManassa.Models;
using System.Text;
using System.Security.Cryptography;
using TadrousManassa.Repositories;
using TadrousManassa.Services;

namespace TadrousManassa.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IStudentRepository _studentRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly DeviceIdentifierService _deviceService;
        private readonly IConfiguration _configuration;

        public LoginModel(IStudentRepository studentRepository, SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger, 
            DeviceIdentifierService deviceService, IConfiguration configuration)
        {
            _studentRepository = studentRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _deviceService = deviceService;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public LoginStudentVM Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Logout
                //await _signInManager.SignOutAsync();
                //return RedirectToAction("Index", "Home", new { area = "Student" });
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home", new { area = "Student" });
            //}
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                // If Admin, Sign in Directly
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Teacher"))
                {
                    var teacherResult = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (teacherResult.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return RedirectToAction("Index", "Home", new { area = "Teacher" });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }

                user.Student = _studentRepository.GetStudent(user.Id);

                //string deviceId = GetDeviceFingerprint();
                string deviceId = _deviceService.GetDeviceId();
                var emailAdmin = _configuration["AdminSimulation:Email"];
                var deviceIdAdmin = _configuration["AdminSimulation:DeviceID"];
                var deviceIdAdmin1 = _configuration["AdminSimulation:ChromeDebugDeviceID"];
                var deviceIdAdmin2 = _configuration["AdminSimulation:BraveDebugDeviceID"];
                if (user.Email != emailAdmin && deviceId != deviceIdAdmin && deviceId != deviceIdAdmin1 && deviceId != deviceIdAdmin2)
                {
                    // Reset DeviceId
                    if (user.Student.DeviceId == "000")
                    {
                        user.Student.DeviceId = deviceId;
                        await _studentRepository.UpdateStudentAsync(user.Student.Id, user.Student);
                    }
                    else if (user.Student.DeviceId != deviceId)
                    {
                        ModelState.AddModelError(string.Empty, "Please login by the same device you have used in registeration.");
                        return Page();
                    }
                }
                
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var studentResult = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                if (studentResult.Succeeded)
                {
                    
                    //if (roles.Contains("Student"))
                    //    return RedirectToAction("Index", "Home", new { area = "Student" });
                    
                    _logger.LogInformation("User logged in.");
                    //return LocalRedirect(returnUrl);
                    return RedirectToAction("Index", "Home", new { area = "Student" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //private string GetDeviceFingerprint(bool isRegistration = false)
        //{
        //    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        //    var deviceIdCookie = HttpContext.Request.Cookies["DeviceId"];

        //    // Generate and persist DeviceId cookie during registration if missing
        //    if (isRegistration && string.IsNullOrEmpty(deviceIdCookie))
        //    {
        //        deviceIdCookie = Guid.NewGuid().ToString();
        //        var cookieOptions = new CookieOptions
        //        {
        //            Expires = DateTime.Now.AddYears(6), // Long-lived cookie
        //            HttpOnly = true,
        //            Secure = true, // Enable if using HTTPS
        //            SameSite = SameSiteMode.Lax // Adjust based on your cross-site needs
        //        };
        //        HttpContext.Response.Cookies.Append("DeviceId", deviceIdCookie, cookieOptions);
        //    }

        //    string rawFingerprint = userAgent;
        //    if (!string.IsNullOrEmpty(deviceIdCookie))
        //    {
        //        rawFingerprint += "-" + deviceIdCookie;
        //    }

        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawFingerprint));
        //        return Convert.ToBase64String(hash);
        //    }
        //}
    }
}
