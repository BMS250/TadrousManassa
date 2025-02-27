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

        public OperationResult<LectureListViewModel> GetLecturesVMWithUnits(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<LectureListViewModel>.Fail("Student ID cannot be null or empty.");

            Student student = studentRepository.GetStudent(studentId);
            if (student == null)
                return OperationResult<LectureListViewModel>.Fail("Student not found.");

            var unitsResult = lectureRepository.GetCurrentUnits(student.Grade);
            if (!unitsResult.Success || unitsResult.Data == null || !unitsResult.Data.Any())
                return OperationResult<LectureListViewModel>.Fail("No units found.");

            var lecturesVM = new Dictionary<string, List<LectureViewModel>>();

            foreach (var unit in unitsResult.Data)
            {
                var lecturesResult = lectureRepository.GetLecturesByUnit(unit);
                if (lecturesResult.Success)
                {
                    var lectureVMList = lecturesResult.Data.Select((lecture, index) => new LectureViewModel
                    {
                        Id = lecture.Id,
                        Name = lecture.Name,
                        ImageURL = $"bg{index + 1}",
                        IsPurchased = IsLecturePurchased(studentId, lecture.Id).Success && IsLecturePurchased(studentId, lecture.Id).Data
                    }).ToList();

                    lecturesVM[unit] = lectureVMList;
                }
                
            }

            var result = new LectureListViewModel { LecturesOfUnits = lecturesVM };

            return OperationResult<LectureListViewModel>.Ok(result, "Lectures with units retrieved successfully.");
        }

        public OperationResult<object> InsertLecture(Lecture lecture)
        {
            return lectureRepository.InsertLecture(lecture);
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            return await lectureRepository.UpdateLectureAsync(id, lecture);
        }

        public Task<OperationResult<object>> DeleteLecture(string id)
        {
            return lectureRepository.DeleteLectureAsync(id);
        }

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId)
        {
            return lectureRepository.IsLecturePurchased(studentId, lectureId);
        }

        public OperationResult<object> BuyCode(string studentId, string code, string lectureId)
        {
            return lectureRepository.BuyCode(studentId, code, lectureId);
        }
    }
}
