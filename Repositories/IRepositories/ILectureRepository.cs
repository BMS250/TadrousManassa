using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface ILectureRepository
    {
        public List<Lecture> GetLectures();

        public List<LectureBasicDTO> GetLecturesBasicData();

        public List<LectureViewsCountDTO> GetLecturesViewsCount();

        public List<Lecture> GetCurrentLectures();

        public int GetViewsCount(string id);

        public OperationResult<Lecture> GetLecture(string id);

        public OperationResult<VideoDetailsDTO> GetVideoDetails(string id, int order);

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade);

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade);

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit);

        public OperationResult<List<string>> GetUnits();

        public OperationResult<List<string>> GetCurrentUnits(int grade);

        public OperationResult<Lecture> AddLecture(Lecture lecture);

        public Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture);

        public Task<OperationResult<bool>> DeleteLectureAsync(string id);
    }
}