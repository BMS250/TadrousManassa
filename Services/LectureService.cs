using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;
using TadrousManassa.Utilities;

namespace TadrousManassa.Services
{
    public class LectureService : ILectureService
    {
        private readonly ILectureRepository _lectureRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentLectureRepository _studentLectureRepository;

        public LectureService(ILectureRepository lectureRepository, IStudentRepository studentRepository, IStudentLectureRepository studentLectureRepository)
        {
            _lectureRepository = lectureRepository;
            _studentRepository = studentRepository;
            _studentLectureRepository = studentLectureRepository;
        }

        public List<Lecture> GetLectures()
        {
            return _lectureRepository.GetLectures();
        }

        public List<BasicDTO> GetLecturesBasicData()
        {
            return _lectureRepository.GetLecturesBasicData();
        }

        public async Task<OperationResult<List<BasicDTO>>> GetLecturesBasicDataByGrade(int grade)
        {
            if (!_lectureRepository.IsValidGrade(grade))
                return OperationResult<List<BasicDTO>>.Fail("Grade must be between 1 and 6");
            var lectures = await _lectureRepository.GetLecturesBasicDataByGrade(grade);
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<BasicDTO>>.Fail($"No current lectures found for grade {grade}.");
            return OperationResult<List<BasicDTO>>.Ok(lectures, "Lectures retrieved successfully.");
        }

        public List<LectureViewsCountDTO> GetLecturesViewsCount()
        {
            return _lectureRepository.GetLecturesViewsCount();
        }

        public List<Lecture> GetCurrentLectures()
        {
            return _lectureRepository.GetCurrentLectures();
        }

        public int GetViewsCount(string id)
        {
            return _lectureRepository.GetViewsCount(id);
        }

        public async Task<OperationResult<Lecture>> GetLecture(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<Lecture>.Fail("Lecture ID cannot be null or empty.");

            var lecture = await _lectureRepository.GetLecture(id);
            if (lecture == null)
                return OperationResult<Lecture>.Fail("Lecture not found.");

            return OperationResult<Lecture>.Ok(lecture, "Lecture retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade)
        {
            return _lectureRepository.GetLecturesByGrade(grade);
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade)
        {
            return _lectureRepository.GetCurrentLecturesByGrade(grade);
        }

        public Task<List<string>> GetLectureNamesByGradeAsync(int grade)
        {
            return _lectureRepository.GetLectureNamesByGradeAsync(grade);
        }

        public OperationResult<List<string>> GetUnits()
        {
            return _lectureRepository.GetUnits();
        }

        public OperationResult<List<string>> GetCurrentUnits(int grade)
        {
            return _lectureRepository.GetCurrentUnits(grade);
        }

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit)
        {
            return _lectureRepository.GetLecturesByUnit(unit);
        }

        public OperationResult<LecturesBySemesterVM> GetLecturesVM(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<LecturesBySemesterVM>.Fail("Student ID cannot be null or empty.");
            Student student = _studentRepository.GetStudent(studentId);
            if (student == null)
                return OperationResult<LecturesBySemesterVM>.Fail("Student not found.");
            var lecturesVM = new Dictionary<int, List<LectureVM>>();
            //var lecturesResult = _lectureRepository.GetLecturesByStudent(studentId);
            var lecturesResult = GetCurrentLecturesByGrade(student.Grade);
            if (lecturesResult.Success)
            {
                int i = 1;
                foreach (var lecture in lecturesResult.Data)
                {
                    var isLecturePurchased = _studentLectureRepository.IsLecturePurchased(studentId, lecture.Id);
                    var lectureVM = new LectureVM
                    {
                        Id = lecture.Id,
                        Name = lecture.Name,
                        ImageURL = $"bg{i++}",
                        IsPurchased = isLecturePurchased.Success && isLecturePurchased.Data
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
            var result = new LecturesBySemesterVM { LecturesOfSemestersByUnits = lecturesVM };
            return OperationResult<LecturesBySemesterVM>.Ok(result, "Lectures retrieved successfully.");
        }

        public OperationResult<Lecture> AddLecture(Lecture lecture)
        {
            return _lectureRepository.AddLecture(lecture);
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            return await _lectureRepository.UpdateLectureAsync(id, lecture);
        }

        public async Task<OperationResult<bool>> DeleteLectureAsync(string id)
        {
            return await _lectureRepository.DeleteLectureAsync(id);
        }
    }
}
