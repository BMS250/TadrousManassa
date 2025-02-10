using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface IStudentRepository
    {
        public Student GetStudent(string id);

        public void InsertStudent(Student student);

        public void UpdateStudent(string Id, Student student);

        public void DeleteStudent(string Id);
    }
}
