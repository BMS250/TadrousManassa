using Microsoft.AspNetCore.Identity;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class LectureRepository : ILectureRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStudentRepository studentRepo;
        private readonly int currentYear;
        private readonly int currentSemester;

        public LectureRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStudentRepository studentRepo, IAppSettingsRepository appSettingsRepo)
        {
            this.context = context;
            this.userManager = userManager;
            this.studentRepo = studentRepo;
            var currentData = appSettingsRepo.GetCurrentData();
            currentYear = currentData.CurrentYear;
            currentSemester = currentData.CurrentSemester;
        }

        public List<Lecture> GetLectures()
        {
            return [.. context.Lectures];
        }

        public List<Lecture> GetCurrentLectures()
        {
            return [.. context.Lectures.Where(l => l.Semester == currentSemester && l.UsedThisYear)];
        }

        public OperationResult<Lecture> GetLecture(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<Lecture>.Fail("Lecture ID cannot be null or empty.");

            var lecture = context.Lectures.FirstOrDefault(l => l.Id == id);
            if (lecture == null)
                return OperationResult<Lecture>.Fail("Lecture not found.");

            return OperationResult<Lecture>.Ok(lecture, "Lecture retrieved successfully.");
        }

        private static bool IsValidGrade(int grade) => grade >= 1 && grade <= 6;

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade)
        {
            if (!IsValidGrade(grade))
                return OperationResult<List<Lecture>>.Fail("Grade must be between 1 and 6");
            var lectures = context.Lectures.Where(l => l.Grade == grade).ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No current lectures found for grade {grade}.");
            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade)
        {
            if (!IsValidGrade(grade))
                return OperationResult<List<Lecture>>.Fail("Grade must be between 1 and 6");
            var lectures = context.Lectures
                .Where(l => /*l.Semester == ApplicationSettings.CurrentSemester && */l.UsedThisYear && l.Grade == grade)
                .ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No current lectures found for grade {grade}.");
            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetLecturesByStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<List<Lecture>>.Fail("Student ID cannot be null or empty.");

            var lectures = context.StudentLectures
                .Where(sl => sl.StudentId == studentId)
                .Select(sl => sl.Lecture)
                .ToList();

            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully for student.");
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<List<Lecture>>.Fail("Student ID cannot be null or empty.");

            var student = context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return OperationResult<List<Lecture>>.Fail("Student not found.");

            var lectures = context.StudentLectures
                .Where(sl => sl.StudentId == studentId)
                .Select(sl => sl.Lecture)
                .Where(l => l.Grade == student.Grade && l.Semester == currentSemester && l.UsedThisYear)
                .ToList();

            return OperationResult<List<Lecture>>.Ok(lectures, "Current lectures retrieved for student.");
        }

        public OperationResult<List<string>> GetUnits()
        {
            var units = context.Lectures.Select(l => l.Unit).Distinct().ToList();
            if (units == null || units.Count == 0)
                return OperationResult<List<string>>.Fail("No units found.");
            return OperationResult<List<string>>.Ok(units, "Units retrieved successfully.");
        }

        public OperationResult<List<string>> GetCurrentUnits(int grade)
        {
            var units = context.Lectures.Where(l => l.Grade == grade && l.Semester == currentSemester && l.UsedThisYear).Select(l => l.Unit).Distinct().ToList();
            if (units == null || units.Count == 0)
                return OperationResult<List<string>>.Fail("No units found.");
            return OperationResult<List<string>>.Ok(units, "Units retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit)
        {
            //if (string.IsNullOrWhiteSpace(unit))
            //    return OperationResult<List<Lecture>>.Fail("Unit cannot be null or empty.");
            //var units = GetUnits();
            //if (units.Success && !units.Data.Contains(unit))
            //    return OperationResult<List<Lecture>>.Fail($"There is not any unit called {unit}.");

            var lectures = context.Lectures.Where(l => l.Unit == unit).ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No lectures found for unit '{unit}'.");
            return OperationResult<List<Lecture>>.Ok(lectures, $"Lectures for unit '{unit}' retrieved successfully.");
        }

        public OperationResult<bool> InsertLecture(Lecture lecture)
        {
            if (lecture == null)
                return OperationResult<bool>.Fail("Lecture cannot be null.");

            context.Lectures.Add(lecture);
            context.SaveChanges();

            return OperationResult<bool>.Ok(true, "Lecture inserted successfully.");
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<int>.Fail("Lecture ID cannot be null or empty.");

            if (lecture == null)
                return OperationResult<int>.Fail("Lecture cannot be null.");

            var existingLecture = context.Lectures.FirstOrDefault(l => l.Id == id);
            if (existingLecture == null)
                return OperationResult<int>.Fail("Lecture not found.");

            context.Entry(existingLecture).CurrentValues.SetValues(lecture);
            try
            {
                await context.SaveChangesAsync();
                return OperationResult<int>.Ok(0, "Lecture updated successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Error updating lecture: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> DeleteLectureAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<bool>.Fail("Lecture ID cannot be null or empty.");

            var lecture = context.Lectures.FirstOrDefault(l => l.Id == id);
            if (lecture == null)
                return OperationResult<bool>.Fail("Lecture not found.");

            try
            {
                context.Lectures.Remove(lecture);
                await context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true, "Lecture deleted successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Error deleting lecture: {ex.Message}");
            }
        }

        public OperationResult<bool> IsLecturePurchased(string studentId, string lectureId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return OperationResult<bool>.Fail("Student ID cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(lectureId))
                return OperationResult<bool>.Fail("Lecture ID cannot be null or empty.");

            bool isPurchased = context.StudentLectures.Any(sl => sl.StudentId == studentId && sl.LectureId == lectureId);
            if (isPurchased)
                return OperationResult<bool>.Ok(true, "Lecture is purchased.");
            return OperationResult<bool>.Ok(false, "Lecture is not purchased.");
        }

        public OperationResult<bool> BuyCode(string studentId, string code, string lectureId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return OperationResult<bool>.Fail("Code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(lectureId))
                return OperationResult<bool>.Fail("Lecture ID cannot be null or empty.");

                StudentLecture? row = context.StudentLectures.FirstOrDefault(sl => sl.Code == code && sl.LectureId == lectureId && (sl.StudentId == "" || sl.StudentId == null));
                if (row == null)
                    return OperationResult<bool>.Fail("Code is not valid.");
            try
            {
                row.StudentId = studentId;
                context.SaveChanges();
                return OperationResult<bool>.Ok(true, "Code is valid.");
            }
            catch
            {
                return OperationResult<bool>.Fail("An Error happened while buying the lecture, please try again.");
            }
        }
    }
}
