using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface ILectureRepository
    {
        public List<Lecture> GetLectures();

        public List<Lecture> GetCurrentLectures(int grade);

        public Lecture GetLecture(string id);

        public List<Lecture> GetLecturesByStudent(string studentId);

        public List<Lecture> GetCurrentLecturesByStudent(string studentId);

        public void InsertLecture(Lecture lecture);

        public Task<int> UpdateLectureAsync(string Id, Lecture lecture);

        public void DeleteLecture(string Id);
    }
}
