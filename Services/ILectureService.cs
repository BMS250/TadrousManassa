using TadrousManassa.Models;
using TadrousManassa.Areas.Student.Models;

namespace TadrousManassa.Services
{
    public interface ILectureService
    {
        public List<Lecture> GetLectures();

        public List<Lecture> GetCurrentLectures();

        public OperationResult<Lecture> GetLecture(string id);

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade);

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade);

        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId);

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId);

        public OperationResult<List<string>> GetUnits();

        public OperationResult<List<string>> GetCurrentUnits(int grade);

        public OperationResult<LectureListViewModel> GetLecturesVMWithUnits(string studentId);

        public OperationResult<object> InsertLecture(Lecture lecture);

        public Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture);

        public Task<OperationResult<object>> DeleteLecture(string id);

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId);
    }
}