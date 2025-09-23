using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IStudentRepository
    {
        public List<Student> GetStudents();
        public Student? GetStudent(string id);
        public Student? GetStudentByEmail(string email);
        public List<Student> GetStudentsByGrade(int grade);
        public int GetStudentGrade(string id);
        public Task<int> GetStudentRank(string id);
        public void InsertStudent(Student student);
        public Task<int> UpdateStudentAsync(string id, Student student, string? newPassword = null);
        public bool UpdateProfileImage(string studentId, byte[] imageBytes);
        public void IncreaseTotalScore(string id, float scoreToAdd);
        public Task ResetDeviceId(string studentEmail);
        public Task<bool> ResetPassword(string studentEmail, string newPassword);
        public void DeleteStudent(string id);
        public Task SaveChangesAsync();
    }
}
