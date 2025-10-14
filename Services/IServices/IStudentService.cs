using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface IStudentService
    {
        OperationResult<List<Student>> GetStudents();
        OperationResult<Student> GetStudent(string id);
        Task<OperationResult<Student?>> GetStudentWithOfflineQuizzesGrades(string id);
        OperationResult<Student> GetStudentByEmail(string email);
        OperationResult<List<Student>> SearchStudents(string query, string type, int limit = 10);
        OperationResult<List<Student>> GetStudentsByGrade(int grade);
        OperationResult<List<Student>> GetStudentsByLecture(string lectureId);
        Task<OperationResult<int>> GetStudentRank(string id);
        Task<OperationResult<bool>> InsertStudentAsync(Student student);
        Task<OperationResult<bool>> UpdateStudentAsync(string id, Student student, string? newPassword = null);
        Task<OperationResult<bool>> ResetDeviceId(string studentEmail);
        Task<OperationResult<bool>> DeleteStudentAsync(string id);
        OperationResult<bool> UpdateProfileImage(string studentId, byte[] imageBytes);
    }

}
