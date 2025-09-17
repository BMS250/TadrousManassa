using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services;
using TadrousManassa.Services.IServices;
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
                //options.User.AllowedUserNameCharacters =
                //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
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

            // In Program.cs or Startup.cs
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 2_147_483_648; // Set a reasonable limit like 2GB
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2_147_483_648; // Same limit for IIS
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2_147_483_648; // Same limit for form uploads
            });


            builder.Services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();
            builder.Services.AddScoped<ICodeRepository, CodeRepository>();
            builder.Services.AddScoped<ICodeService, CodeService>();
            builder.Services.AddScoped<ILectureRepository, LectureRepository>();
            builder.Services.AddScoped<ILectureService, LectureService>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IStudentChoiceRepository, StudentChoiceRepository>();
            builder.Services.AddScoped<IStudentChoiceService, StudentChoiceService>();
            builder.Services.AddScoped<IStudentLectureRepository, StudentLectureRepository>();
            builder.Services.AddScoped<IStudentLectureService, StudentLectureService>();
            builder.Services.AddScoped<IStudentQuizRepository, StudentQuizRepository>();
            builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IVideoRepository, VideoRepository>();
            builder.Services.AddScoped<IVideoService, VideoService>();

            // Email Service
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            // Add HttpContextAccessor (required for accessing the current HTTP context)
            builder.Services.AddHttpContextAccessor();

            // Register your custom DeviceIdentifierService
            builder.Services.AddScoped<DeviceIdentifierService>();

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

            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
                await next();
            });


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
