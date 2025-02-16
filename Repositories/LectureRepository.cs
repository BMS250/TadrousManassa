using Microsoft.AspNetCore.Identity;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class LectureRepository : ILectureRepository
    {
        private readonly ApplicationDbContext context;
        //private readonly UserManager<ApplicationUser> userManager;
        private readonly IStudentRepository studentRepo;

        public LectureRepository(ApplicationDbContext context/*, UserManager<ApplicationUser> userManager*/, IStudentRepository studentRepo)
        {
            this.context = context;
            //this.userManager = userManager;
            this.studentRepo = studentRepo;
        }

        public List<Lecture> GetLectures()
        {
            return context.Lectures.ToList();
        }

        public List<Lecture> GetCurrentLectures(int grade)
        {
            return context.Lectures.Where(l => l.Grade == grade && l.Semester == Utilities.CurrentSemester).ToList();
        }

        public Lecture GetLecture(string id)
        {
            return context.Lectures.FirstOrDefault(l => l.Id == id);
        }

        public List<Lecture> GetLecturesByStudent(string studentId)
        {
            return context.StudentLectures.Where(sl => sl.StudentId == studentId).Select(sl => sl.Lecture).ToList();
        }

        public List<Lecture> GetCurrentLecturesByStudent(string studentId)
        {
            return context.StudentLectures.Where(sl => sl.StudentId == studentId).Select(sl => sl.Lecture).Where(l => l.Grade == studentRepo.GetStudentGrade(studentId) && l.Semester == Utilities.CurrentSemester).ToList();
        }

        public void InsertLecture(Lecture lecture)
        {
            context.Lectures.Add(lecture);
            context.SaveChanges();
        }

        public async Task<int> UpdateLectureAsync(string Id, Lecture lecture)
        {
            Lecture oldLecture = context.Lectures.FirstOrDefault(l => l.Id == Id);
            if (oldLecture == null)
            {
                return 1; // Lecture not found
            }
            context.Entry(oldLecture).CurrentValues.SetValues(lecture);
            await context.SaveChangesAsync();
            return 0;
        }

        public void DeleteLecture(string Id)
        {
            Lecture lecture = context.Lectures.FirstOrDefault(l => l.Id == Id);
            if (lecture != null)
            {
                context.Lectures.Remove(lecture);
                context.SaveChanges();
            }
        }


    }
}
