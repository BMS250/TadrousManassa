using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public interface IStudentService
    {
        OperationResult<List<Student>> GetStudents();
        OperationResult<Student> GetStudent(string id);
        OperationResult<List<Student>> GetStudentsByGrade(int grade);
        OperationResult<List<Student>> GetStudentsByLecture(string lectureId);
        Task<OperationResult<bool>> InsertStudentAsync(Student student);
        Task<OperationResult<bool>> UpdateStudentAsync(string id, Student student, string? newPassword = null);
        Task<OperationResult<bool>> DeleteStudentAsync(string id);
    }

}
