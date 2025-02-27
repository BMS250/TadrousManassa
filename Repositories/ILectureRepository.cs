using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface ILectureRepository
    {
        public List<Lecture> GetLectures();

        public List<Lecture> GetCurrentLectures();

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade);

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade);

        public OperationResult<Lecture> GetLecture(string id);

        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId);

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId);

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit);

        public OperationResult<List<string>> GetUnits();

        public OperationResult<List<string>> GetCurrentUnits(int grade);

        public OperationResult<object> InsertLecture(Lecture lecture);

        public Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture);

        public Task<OperationResult<object>> DeleteLectureAsync(string id);

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId);

        public OperationResult<object> BuyCode(string studentId, string code, string lectureId);
    }
}