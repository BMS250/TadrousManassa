using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface IStudentRepository
    {
        public List<Student> GetStudents();

        public Student? GetStudent(string id);

        public Student? GetStudentByEmail(string email);

        public List<Student> GetStudentsByGrade(int grade);

        public int GetStudentGrade(string id);

        public void InsertStudent(Student student);

        public Task<int> UpdateStudentAsync(string id, Student student, string? newPassword = null);
        
        public Task ResetDeviceId(string studentEmail);

        public Task<bool> ResetPassword(string studentEmail, string newPassword);

        public void DeleteStudent(string id);
    }
}
