using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Web.Helpers;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public StudentRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        public List<Student> GetStudents()
        {
            return [.. _context.Students];
        }

        public Student? GetStudent(string id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }

        public Student? GetStudentByEmail(string email)
        {
            return _context.Students.FirstOrDefault(s => s.ApplicationUser.Email == email);
        }

        public List<Student> GetStudentsByGrade(int grade)
        {
            return [.. _context.Students.Where(s => s.Grade == grade)];
        }

        public int GetStudentGrade(string id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id).Grade;
        }
        public async Task<int> GetStudentRank(string id)
        {
            // 1. Calculate total score per student
            var studentScores = await _context.Students
                .Select(s => new
                {
                    StudentId = s.Id,
                    s.TotalScore
                })
                .Distinct()
                .OrderByDescending(s => s.TotalScore)
                .ToListAsync();

            // 2. Find the rank
            var rank = studentScores
                .Select((s, index) => new { s.StudentId, Rank = index + 1 })
                .FirstOrDefault(x => x.StudentId == id);

            return rank?.Rank ?? 0;
        }


        public void InsertStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public async Task<int> UpdateStudentAsync(string id, Student student, string? newPassword = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Student id cannot be null or empty", nameof(id));
            ArgumentNullException.ThrowIfNull(student);
            Student oldStudent = GetStudent(id) ?? throw new ArgumentException("Student not found", nameof(id));
            if (oldStudent.ApplicationUser != null && student.ApplicationUser != null)
            {
                oldStudent.ApplicationUser.UserName = student.ApplicationUser.UserName;
                oldStudent.ApplicationUser.Email = student.ApplicationUser.Email;
                oldStudent.ApplicationUser.PhoneNumber = student.ApplicationUser.PhoneNumber;

                // If a new password is provided, update it securely using UserManager
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(oldStudent.ApplicationUser);
                    var result = await userManager.ResetPasswordAsync(oldStudent.ApplicationUser, token, newPassword);

                    if (!result.Succeeded)
                    {
                        return 2; // Password update failed
                    }
                }

                _context.Entry(oldStudent.ApplicationUser).State = EntityState.Modified; // Ensure it's tracked
            }

            // Update student-specific properties
            oldStudent.PhoneNumber_Parents = student.PhoneNumber_Parents;
            oldStudent.Grade = student.Grade;
            oldStudent.Address = student.Address;
            oldStudent.School = student.School;
            oldStudent.ReferralSource = student.ReferralSource;

            await _context.SaveChangesAsync();
            return 0;
        }

        public async Task ResetDeviceId(string studentEmail)
        {
            if (string.IsNullOrEmpty(studentEmail))
                throw new ArgumentException("Student email cannot be null or empty", nameof(studentEmail));

            Student? student = GetStudentByEmail(studentEmail) ?? throw new ArgumentException("Student cannot be null");
            student.DeviceId = "000";
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("Email or new password is empty.");
                return false;
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return false;
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                Console.WriteLine("Password reset successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to reset password:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Code}: {error.Description}");
                }
                return false;
            }
        }

        public void DeleteStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Student id cannot be null or empty", nameof(id));
            Student student = GetStudent(id);
            ArgumentNullException.ThrowIfNull(student);
            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        public bool UpdateProfileImage(string studentId, byte[] imageBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                    return false;

                if (imageBytes == null || imageBytes.Length == 0)
                    return false;

                var student = GetStudent(studentId);
                if (student == null)
                    return false;

                student.ProfileImage = imageBytes;
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
