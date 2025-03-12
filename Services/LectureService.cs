using System.Collections.Generic;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Utilities;

namespace TadrousManassa.Services
{
    public class LectureService : ILectureService
    {
        private readonly ILectureRepository lectureRepository;
        private readonly IStudentRepository studentRepository;

        public LectureService(ILectureRepository lectureRepository, IStudentRepository studentRepository)
        {
            this.lectureRepository = lectureRepository;
            this.studentRepository = studentRepository;
        }

        public List<Lecture> GetLectures()
        {
            return lectureRepository.GetLectures();
        }

        public List<Lecture> GetCurrentLectures()
        {
            return lectureRepository.GetCurrentLectures();
        }

        public int GetViewsCount(string id)
        {
            return lectureRepository.GetViewsCount(id);
        }

        public Dictionary<string, int> GetNoWatchers()
        {
            return lectureRepository.GetNoWatchers();
        }

        public OperationResult<int> IncrementViewsCount(string id)
        {
            return lectureRepository.IncrementViewsCount(id);
        }

        public OperationResult<int> MarkAsWatched(string studentId, string lectureId)
        {
            return lectureRepository.MarkAsWatched(studentId, lectureId);
        }

        public OperationResult<Lecture> GetLecture(string id)
        {
            return lectureRepository.GetLecture(id);
        }

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade)
        {
            return lectureRepository.GetLecturesByGrade(grade);
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade)
        {
            return lectureRepository.GetCurrentLecturesByGrade(grade);
        }

        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId)
        {
            return lectureRepository.GetLecturesByStudent(studentId);
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId)
        {
            return lectureRepository.GetCurrentLecturesByStudent(studentId);
        }

        public OperationResult<List<string>> GetUnits()
        {
            return lectureRepository.GetUnits();
        }

        public OperationResult<List<string>> GetCurrentUnits(int grade)
        {
            return lectureRepository.GetCurrentUnits(grade);
        }

        public OperationResult<LectureListViewModel> GetLecturesVM(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<LectureListViewModel>.Fail("Student ID cannot be null or empty.");
            Student student = studentRepository.GetStudent(studentId);
            if (student == null)
                return OperationResult<LectureListViewModel>.Fail("Student not found.");
            var lecturesVM = new Dictionary<int, List<LectureViewModel>>();
            //var lecturesResult = lectureRepository.GetLecturesByStudent(studentId);
            var lecturesResult = lectureRepository.GetCurrentLecturesByGrade(student.Grade);
            if (lecturesResult.Success)
            {
                int i = 1;
                foreach (var lecture in lecturesResult.Data)
                {
                    var lectureVM = new LectureViewModel
                    {
                        Id = lecture.Id,
                        Name = lecture.Name,
                        ImageURL = $"bg{i++}",
                        IsPurchased = IsLecturePurchased(studentId, lecture.Id).Success && IsLecturePurchased(studentId, lecture.Id).Data
                    };
                    try
                    {
                        lecturesVM[lecture.Semester].Add(lectureVM);
                    }
                    catch
                    {
                        lecturesVM[lecture.Semester] = [lectureVM];
                    }
                }
            }
            var result = new LectureListViewModel { LecturesOfSemesters = lecturesVM };
            return OperationResult<LectureListViewModel>.Ok(result, "Lectures retrieved successfully.");
        }

        public OperationResult<Lecture> AddLecture(Lecture lecture)
        {
            return lectureRepository.AddLecture(lecture);
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            return await lectureRepository.UpdateLectureAsync(id, lecture);
        }

        public Task<OperationResult<bool>> DeleteLecture(string id)
        {
            return lectureRepository.DeleteLectureAsync(id);
        }

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId)
        {
            return lectureRepository.IsLecturePurchased(studentId, lectureId);
        }

        public OperationResult<bool> BuyCode(string studentId, string code, string lectureId)
        {
            return lectureRepository.BuyCode(studentId, code, lectureId);
        }
    }
}
