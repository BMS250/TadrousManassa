using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class StudentLectureService : IStudentLectureService
    {
        private readonly IStudentLectureRepository _studentLectureRepository;
        private readonly ICodeService _codeService;

        public StudentLectureService(IStudentLectureRepository studentLectureRepository, ICodeService codeService)
        {
            _studentLectureRepository = studentLectureRepository;
            _codeService = codeService;
        }

        public OperationResult<bool> BuyCode(string studentId, string lectureId, string code)
        {
            var result = _studentLectureRepository.BuyCode(studentId, lectureId, code);
            CheckRemainingCodes(lectureId);
            return result;
        }

        public int GetRemainingCodes(string lectureId)
        {
            return _studentLectureRepository.GetRemainingCodes(lectureId);
        }

        public void CheckRemainingCodes(string lectureId)
        {
            _studentLectureRepository.CheckRemainingCodes(lectureId);
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId)
        {
            return _studentLectureRepository.GetCurrentLecturesByStudent(studentId);
        }

        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId)
        {
            return _studentLectureRepository.GetLecturesByStudent(studentId);
        }

        public Dictionary<string, int> GetNoWatchers()
        {
            return _studentLectureRepository.GetNoWatchers();
        }

        public List<Student> GetStudentsByLecture(string lectureId)
        {
            return _studentLectureRepository.GetStudentsByLecture(lectureId);
        }

        public Dictionary<string, Dictionary<string, int>> GetViewsCountForStudents()
        {
            return _studentLectureRepository.GetViewsCountForStudents();
        }

        public OperationResult<object> IncrementViewsCount(string studentId, string lectureId)
        {
            return _studentLectureRepository.IncrementViewsCount(studentId, lectureId);
        }

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId)
        {
            return _studentLectureRepository.IsLecturePurchased(studentId, lectureId);
        }

        public OperationResult<bool> MarkCodeAsSold(string lectureId, string code)
        {
            return _studentLectureRepository.MarkCodeAsSold(lectureId, code);
        }
    }
}
