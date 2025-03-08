using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Services;
using TadrousManassa.Utilities;

namespace TadrousManassa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database Configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity Configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = null; // Allows all characters, including duplicates
                options.Password.RequireDigit = false;       // No numbers required
                options.Password.RequireNonAlphanumeric = false; // No symbols required
                options.Password.RequireUppercase = false;  // No uppercase required
                options.Password.RequireLowercase = false;  // No lowercase required
                options.Password.RequiredUniqueChars = 0;   // No unique chars required
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //.AddDefaultTokenProviders(); // This enables email confirmation & password reset

            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ILectureRepository, LectureRepository>();
            builder.Services.AddScoped<ICodeRepository, CodeRepository>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ILectureService, LectureService>();
            builder.Services.AddScoped<ICodeService, CodeService>();
            // Email Service
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            // Add HttpContextAccessor (required for accessing the current HTTP context)
            builder.Services.AddHttpContextAccessor();

            // Register your custom DeviceIdentifierService
            builder.Services.AddScoped<DeviceIdentifierService>();

            builder.Services.AddAWSService<IAmazonS3>();

            // MVC & Razor Pages
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Configure Identity
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // Add to your services configuration
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Middleware Configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Student" });
            // This should come AFTER the areas route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Student" });

            app.MapRazorPages();

            app.Run();
        }
    }
}
