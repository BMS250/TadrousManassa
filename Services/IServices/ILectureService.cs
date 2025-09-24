using TadrousManassa.Models;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;

namespace TadrousManassa.Services.IServices
{
    public interface ILectureService
    {
        public List<Lecture> GetLectures();
        public List<BasicDTO> GetLecturesBasicData();
        public Task<OperationResult<List<BasicDTO>>> GetLecturesBasicDataByGrade(int grade);
        public List<LectureViewsCountDTO> GetLecturesViewsCount();
        public List<Lecture> GetCurrentLectures();
        public int GetViewsCount(string id);
        public Task<OperationResult<Lecture>> GetLecture(string id);
        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade);
        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade);
        public OperationResult<List<string>> GetUnits();
        public OperationResult<List<string>> GetCurrentUnits(int grade);
        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit);
        public OperationResult<LecturesBySemesterVM> GetLecturesVM(string studentId);
        public OperationResult<Lecture> AddLecture(Lecture lecture);
        public Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture);
        public Task<OperationResult<bool>> DeleteLectureAsync(string id);
    }
}