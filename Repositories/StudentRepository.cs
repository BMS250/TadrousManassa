using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public StudentRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public List<Student> GetStudents()
        {
            return context.Students.ToList();
        }

        public Student GetStudent(string id)
        {
            return context.Students.FirstOrDefault(s => s.Id == id);
        }

        public List<Student> GetStudentsByGrade(int grade)
        {
            return context.Students.Where(s => s.Grade == grade).ToList();
        }

        public List<Student> GetStudentsByLecture(string lectureId)
        {
            return context.StudentLectures.Where(sl => sl.LectureId == lectureId).Select(sl => sl.Student).ToList();
        }

        public int GetStudentGrade(string studentId)
        {
            return context.Students.FirstOrDefault(s => s.Id == studentId).Grade;
        }

        public void InsertStudent(Student student)
        {
            context.Students.Add(student);
            context.SaveChanges();
        }

        public async Task<int> UpdateStudentAsync(string Id, Student student, string? newPassword = null)
        {
            Student oldStudent = GetStudent(Id);
            if (oldStudent == null)
            {
                return 1; // Student not found
            }

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

                context.Entry(oldStudent.ApplicationUser).State = EntityState.Modified; // Ensure it's tracked
            }

            // Update student-specific properties
            oldStudent.PhoneNumber_Parents = student.PhoneNumber_Parents;
            oldStudent.Grade = student.Grade;
            oldStudent.Address = student.Address;
            oldStudent.School = student.School;
            oldStudent.ReferralSource = student.ReferralSource;

            await context.SaveChangesAsync();
            return 0;
        }

        public void DeleteStudent(string Id)
        {
            Student student = GetStudent(Id);
            if (student != null)
            {
                context.Students.Remove(student);
                context.SaveChanges();
            }
        }
    }
}
