// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Services;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DeviceIdentifierService _deviceService;

        public RegisterModel(
            IStudentService studentService,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            DeviceIdentifierService deviceService)
        {
            _studentService = studentService;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _deviceService = deviceService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public RegisterStudentVM Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            return NotFound();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Student" });
            }
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Name, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.Student = new Models.Student
                {
                    Address = Input.Address,
                    Grade = Input.Grade,
                    PhoneNumber_Parents = Input.PhoneNumber_Parents,
                    ReferralSource = Input.ReferralSource,
                    School = Input.School,
                    //DeviceId = GetDeviceFingerprint(isRegistration: true)
                    DeviceId = _deviceService.GetDeviceId()
                };
                user.PhoneNumber = Input.PhoneNumber;
                var result = await _userManager.CreateAsync(user, Input.Password);
                await _userManager.AddToRoleAsync(user, "Student");
                if (result.Succeeded)
                {
                    //    _logger.LogInformation("User created a new account with password.");

                    //    // Call the StudentService to insert the student information
                    //    var studentResult = await _studentService.InsertStudentAsync(user.Student);
                    //    if (!studentResult.Success)
                    //    {
                    //        // If the student insertion failed, add the error message to ModelState
                    //        ModelState.AddModelError(string.Empty, studentResult.Message);
                    //        return Page();
                    //    }

                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    //return LocalRedirect("~/");
                    //    return RedirectToAction("Index", "Home", new { area = "Student" });
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new { area = "Student" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
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
