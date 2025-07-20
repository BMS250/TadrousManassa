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
        private readonly ILectureRepository lectureRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IStudentLectureRepository studentLectureRepository;

        public LectureService(ILectureRepository lectureRepository, IStudentRepository studentRepository, IStudentLectureRepository studentLectureRepository)
        {
            this.lectureRepository = lectureRepository;
            this.studentRepository = studentRepository;
            this.studentLectureRepository = studentLectureRepository;
        }

        public List<Lecture> GetLectures()
        {
            return lectureRepository.GetLectures();
        }

        public List<LectureBasicDTO> GetLecturesBasicData()
        {
            return lectureRepository.GetLecturesBasicData();
        }
        public List<LectureViewsCountDTO> GetLecturesViewsCount()
        {
            return lectureRepository.GetLecturesViewsCount();
        }

        public List<Lecture> GetCurrentLectures()
        {
            return lectureRepository.GetCurrentLectures();
        }

        public int GetViewsCount(string id)
        {
            return lectureRepository.GetViewsCount(id);
        }

        public OperationResult<Lecture> GetLecture(string id)
        {
            return lectureRepository.GetLecture(id);
        }

        public OperationResult<VideoDetailsDTO> GetVideoDetails(string id, int order)
        {
            return lectureRepository.GetVideoDetails(id, order);
        }

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade)
        {
            return lectureRepository.GetLecturesByGrade(grade);
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade)
        {
            return lectureRepository.GetCurrentLecturesByGrade(grade);
        }

        public OperationResult<List<string>> GetUnits()
        {
            return lectureRepository.GetUnits();
        }

        public OperationResult<List<string>> GetCurrentUnits(int grade)
        {
            return lectureRepository.GetCurrentUnits(grade);
        }

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit)
        {
            return lectureRepository.GetLecturesByUnit(unit);
        }

        public OperationResult<LecturesBySemesterVM> GetLecturesVM(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<LecturesBySemesterVM>.Fail("Student ID cannot be null or empty.");
            Student student = studentRepository.GetStudent(studentId);
            if (student == null)
                return OperationResult<LecturesBySemesterVM>.Fail("Student not found.");
            var lecturesVM = new Dictionary<int, List<LectureVM>>();
            //var lecturesResult = lectureRepository.GetLecturesByStudent(studentId);
            var lecturesResult = lectureRepository.GetCurrentLecturesByGrade(student.Grade);
            if (lecturesResult.Success)
            {
                int i = 1;
                foreach (var lecture in lecturesResult.Data)
                {
                    var isLecturePurchased = studentLectureRepository.IsLecturePurchased(studentId, lecture.Id);
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
            return lectureRepository.AddLecture(lecture);
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            return await lectureRepository.UpdateLectureAsync(id, lecture);
        }

        public async Task<OperationResult<bool>> DeleteLectureAsync(string id)
        {
            return await lectureRepository.DeleteLectureAsync(id);
        }
    }
}
