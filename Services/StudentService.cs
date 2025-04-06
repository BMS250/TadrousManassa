using Microsoft.IdentityModel.Tokens;
using System.Text;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository studentRepository;
        private readonly ILogger<StudentService> logger;
        private readonly IStudentLectureRepository studentLectureRepository;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger, IStudentLectureRepository studentLectureRepository)
        {
            this.studentRepository = studentRepository;
            this.logger = logger;
            this.studentLectureRepository = studentLectureRepository;
        }

        public OperationResult<List<Student>> GetStudents()
        {
            try
            {
                var students = studentRepository.GetStudents();
                return OperationResult<List<Student>>.Ok(students);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving students.");
                return OperationResult<List<Student>>.Fail("Failed to retrieve students.");
            }
        }

        public OperationResult<List<Student>> GetStudentsByGrade(int grade)
        {
            if (grade < 1 || grade > 6)
                return OperationResult<List<Student>>.Fail("Grade must be between 1 and 6");

            var students = studentRepository.GetStudentsByGrade(grade);
            return OperationResult<List<Student>>.Ok(students);
        }

        public OperationResult<List<Student>> GetStudentsByLecture(string lectureId)
        {
            if (string.IsNullOrEmpty(lectureId))
                return OperationResult<List<Student>>.Fail("Lecture id cannot be null or empty");

            var students = studentLectureRepository.GetStudentsByLecture(lectureId);
            return OperationResult<List<Student>>.Ok(students);
        }

        public OperationResult<Student> GetStudent(string id)
        {
            try
            {
                var student = studentRepository.GetStudent(id);
                if (student == null)
                    return OperationResult<Student>.Fail("Student not found.");

                return OperationResult<Student>.Ok(student);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving student with ID {id}");
                return OperationResult<Student>.Fail("Failed to retrieve student.");
            }
        }
        public OperationResult<Student> GetStudentByEmail(string email)
        {
            try
            {
                var student = studentRepository.GetStudentByEmail(email);
                if (student == null)
                    return OperationResult<Student>.Fail("Student not found.");
                return OperationResult<Student>.Ok(student);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving student with email {email}");
                return OperationResult<Student>.Fail("Failed to retrieve student.");
            }
        }

        public async Task<OperationResult<bool>> InsertStudentAsync(Student student)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(student);
                studentRepository.InsertStudent(student);
                return OperationResult<bool>.Ok(true, "Student inserted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting student.");
                return OperationResult<bool>.Fail("Failed to insert student.");
            }
        }

        public async Task<OperationResult<bool>> UpdateStudentAsync(string id, Student student, string? newPassword = null)
        {
            try
            {
                var result = await studentRepository.UpdateStudentAsync(id, student, newPassword);
                if (result == 0)
                    return OperationResult<bool>.Ok(true, "Student updated successfully.");

                return OperationResult<bool>.Fail("Failed to update student.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating student with ID {id}");
                return OperationResult<bool>.Fail("Failed to update student.");
            }
        }

        public async Task<OperationResult<bool>> ResetDeviceId(string studentEmail)
        {
            try
            {
                await studentRepository.ResetDeviceId(studentEmail);
                return OperationResult<bool>.Ok(true, "Device ID reset successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error resetting device ID for student with Email {studentEmail}");
                return OperationResult<bool>.Fail("Failed to reset device ID.");
            }
        }

        public async Task<OperationResult<bool>> DeleteStudentAsync(string id)
        {
            try
            {
                studentRepository.DeleteStudent(id);
                return OperationResult<bool>.Ok(true, "Student deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting student with ID {id}");
                return OperationResult<bool>.Fail("Failed to delete student.");
            }
        }

        
    }

}
