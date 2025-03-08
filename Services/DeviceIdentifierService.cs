using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;

namespace TadrousManassa.Services
{

    public class DeviceIdentifierService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeviceIdentifierService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetDeviceId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var deviceIdCookie = httpContext.Request.Cookies["DeviceId"];

            // If the cookie exists, return it directly
            if (!string.IsNullOrEmpty(deviceIdCookie))
            {
                return deviceIdCookie;
            }

            // If no cookie exists, check for a client-stored ID (e.g., localStorage)
            var clientDeviceId = httpContext.Request.Headers["X-Device-Id"].ToString();

            if (!string.IsNullOrEmpty(clientDeviceId))
            {
                // Validate and set the cookie from the client's storage
                SetDeviceIdCookie(clientDeviceId);
                return clientDeviceId;
            }

            // Generate a new ID if no existing ID is found
            var newDeviceId = GenerateStableDeviceId();
            SetDeviceIdCookie(newDeviceId);
            return newDeviceId;
        }

        private void SetDeviceIdCookie(string deviceId)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(6), // Long-lived cookie
                HttpOnly = true, // Secure against client-side tampering
                Secure = true, // Enable if using HTTPS
                SameSite = SameSiteMode.Lax // Adjust based on cross-site needs
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("DeviceId", deviceId, cookieOptions);
        }

        private string GenerateStableDeviceId()
        {
            // Generate a unique GUID-based ID (customize as needed)
            return Guid.NewGuid().ToString("N");
        }
    }
}