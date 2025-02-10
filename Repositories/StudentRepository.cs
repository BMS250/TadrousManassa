using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext context;

        public StudentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Student GetStudent(string id)
        {
            return context.Students.FirstOrDefault(s => s.Id == id);
        }

        public void InsertStudent(Student student)
        {
            throw new NotImplementedException();
        }

        public void UpdateStudent(string Id, Student student)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudent(string Id)
        {
            throw new NotImplementedException();
        }

    }
}
