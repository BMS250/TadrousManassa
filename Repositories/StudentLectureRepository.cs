using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class StudentLectureRepository : IStudentLectureRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStudentRepository studentRepo;
        private readonly ICodeRepository codeRepository;
        private readonly int currentYear;
        private readonly int currentSemester;

        public StudentLectureRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStudentRepository studentRepo, IAppSettingsRepository appSettingsRepo, ICodeRepository codeRepository)
        {
            this._context = context;
            this.userManager = userManager;
            this.studentRepo = studentRepo;
            this.codeRepository = codeRepository;
            var currentData = appSettingsRepo.GetCurrentData();
            currentYear = currentData.CurrentYear;
            currentSemester = currentData.CurrentSemester;
        }


        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<List<Lecture>>.Fail("Student ID cannot be null or empty.");

            var lectures = _context.StudentLectures
                .Where(sl => sl.StudentId == studentId)
                .Select(sl => sl.Lecture)
                .ToList();

            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully for student.");
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<List<Lecture>>.Fail("Student ID cannot be null or empty.");

            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return OperationResult<List<Lecture>>.Fail("Student not found.");

            var lectures = _context.StudentLectures
                .Where(sl => sl.StudentId == studentId)
                .Select(sl => sl.Lecture)
                .Where(l => l.Grade == student.Grade && l.Semester == currentSemester && l.UsedThisYear)
                .ToList();

            return OperationResult<List<Lecture>>.Ok(lectures, "Current lectures retrieved for student.");
        }

        public List<Student> GetStudentsByLecture(string lectureId)
        {
            return _context.StudentLectures.Where(sl => sl.LectureId == lectureId).Select(sl => sl.Student).ToList();
        }

        public Dictionary<string, Dictionary<string, int>> GetViewsCountPerStudents()
        {
            Dictionary<string, Dictionary<string, int>> ViewsCountPerStudents = [];
            foreach (var studentLecture in _context.StudentLectures
                                            .Where(sl => !string.IsNullOrEmpty(sl.StudentId)))
            {
                var student = userManager.Users.FirstOrDefault(u => u.Id == studentLecture.StudentId);
                if (student == null)
                    continue;

                string studentName = student.UserName;

                // Ensure the dictionary entry exists
                if (!ViewsCountPerStudents.TryGetValue(studentLecture.LectureId, out var studentViews))
                {
                    studentViews = new Dictionary<string, int>();
                    ViewsCountPerStudents[studentLecture.LectureId] = studentViews;
                }

                // Update the student's view count
                studentViews[studentName] = studentLecture.ViewsCount;

            }
            return ViewsCountPerStudents;
        }

        public Dictionary<string, int> GetNoWatchers()
        {
            Dictionary<string, int> noWatcheres = [];
            foreach (var studentLecture in _context.StudentLectures)
            {
                noWatcheres[studentLecture.LectureId] = _context.StudentLectures.Count(sl => sl.LectureId == studentLecture.LectureId
                                                                                        && sl.IsWatched);
            }
            return noWatcheres;
        }

        public OperationResult<object> IncrementViewsCount(string studentId, string lectureId)
        {
            var lecture = _context.Lectures.FirstOrDefault(l => l.Id == lectureId);
            if (lecture == null)
                return OperationResult<object>.Fail("Lecture not found.");
            lecture.ViewsCount++;
            var studentLecture = _context.StudentLectures.FirstOrDefault(sl => sl.StudentId == studentId && sl.LectureId == lectureId);
            if (studentLecture == null)
                return OperationResult<object>.Fail("Student lecture not found.");
            studentLecture.IsWatched = true;
            studentLecture.ViewsCount++;
            try
            {
                _context.SaveChanges();
                return OperationResult<object>.Ok(new { lectureViewsCount = lecture.ViewsCount, studentViewsCount = studentLecture.ViewsCount }, "Views count incremented successfully.");
            }
            catch (Exception)
            {
                return OperationResult<object>.Fail("Views count didn't increment.");
            }
        }

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<bool>.Fail("Student ID cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(lectureId))
                return OperationResult<bool>.Fail("Lecture ID cannot be null or empty.");

            bool isPurchased = _context.StudentLectures.Any(sl => sl.StudentId == studentId && sl.LectureId == lectureId);
            if (isPurchased)
                return OperationResult<bool>.Ok(true, "Lecture is purchased.");
            return OperationResult<bool>.Ok(false, "Lecture is not purchased.");
        }

        public OperationResult<bool> BuyCode(string studentId, string lectureId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return OperationResult<bool>.Fail("Code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(lectureId))
                return OperationResult<bool>.Fail("Lecture ID cannot be null or empty.");

            StudentLecture? row = _context.StudentLectures.FirstOrDefault(sl => sl.Code == code && sl.LectureId == lectureId && (sl.StudentId == "" || sl.StudentId == null));
            if (row == null)
                return OperationResult<bool>.Fail("Code is not valid.");
            try
            {
                row.StudentId = studentId;
                _context.SaveChanges();
                return OperationResult<bool>.Ok(true, "Code is valid.");
            }
            catch
            {
                return OperationResult<bool>.Fail("An Error happened while buying the lecture, please try again.");
            }
        }

        public int GetRemainingCodes(string lectureId)
        {
            return _context.StudentLectures.Count(sl => sl.LectureId == lectureId && (sl.StudentId == "" || sl.StudentId == null));
        }

        public void CheckRemainingCodes(string lectureId)
        {
            int remainingCodes = GetRemainingCodes(lectureId);
            if (remainingCodes < 10)
            {
                codeRepository.GenerateCodes(100 - remainingCodes, lectureId);
            }
        }

        public OperationResult<bool> MarkCodeAsSold(string lectureId, string code)
        {
            try
            {
                var studentLecture = _context.StudentLectures
                    .Where(sl => sl.LectureId == lectureId && sl.Code == code).FirstOrDefault()!;
                studentLecture.IsSold = true;
                _context.SaveChanges();
                return OperationResult<bool>.Ok(true, "Data updated successfully");
            }
            catch
            {
                return OperationResult<bool>.Fail("Failed to update data");
            }
        }
    }
}
