using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface IStudentRepository
    {
        public List<Student> GetStudents();

        public Student? GetStudent(string id);

        public List<Student> GetStudentsByGrade(int grade);

        public List<Student> GetStudentsByLecture(string lectureId);

        public int GetStudentGrade(string studentId);

        public void InsertStudent(Student student);

        public Task<int> UpdateStudentAsync(string Id, Student student, string? newPassword = null);

        public void DeleteStudent(string Id);
    }
}
