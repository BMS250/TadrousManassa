using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface ILectureRepository
    {
        public List<Lecture> GetLectures();
        public List<BasicDTO> GetLecturesBasicData();
        public Task<List<BasicDTO>> GetLecturesBasicDataByGrade(int grade);
        public List<LectureViewsCountDTO> GetLecturesViewsCount();
        public List<Lecture> GetCurrentLectures();
        public int GetViewsCount(string id);
        public Task<Lecture?> GetLecture(string id);
        public Task<string?> GetUnit(string id);
        public bool IsValidGrade(int grade);
        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade);
        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade);
        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit);
        public Task<List<string>> GetLectureNamesByGradeAsync(int grade);
        public OperationResult<List<string>> GetUnits();
        public OperationResult<List<string>> GetCurrentUnits(int grade);
        public OperationResult<Lecture> AddLecture(Lecture lecture);
        public Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture);
        public Task<OperationResult<bool>> DeleteLectureAsync(string id);
    }
}