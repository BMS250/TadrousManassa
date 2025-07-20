using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Areas.Teacher.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class LectureRepository : ILectureRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IStudentRepository studentRepo;
        private readonly int currentYear;
        private readonly int currentSemester;

        public LectureRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStudentRepository studentRepo, IAppSettingsRepository appSettingsRepo)
        {
            _context = context;
            this.userManager = userManager;
            this.studentRepo = studentRepo;
            var currentData = appSettingsRepo.GetCurrentData();
            currentYear = currentData.CurrentYear;
            currentSemester = currentData.CurrentSemester;
        }

        public List<Lecture> GetLectures()
        {
            return [.. _context.Lectures.AsNoTracking()];
        }

        public List<LectureBasicDTO> GetLecturesBasicData()
        {
            return [.. _context.Lectures.AsNoTracking().Select(l => new LectureBasicDTO { Id = l.Id, Name = l.Name })];
        }

        public List<LectureViewsCountDTO> GetLecturesViewsCount()
        {
            return [.. _context.Lectures.AsNoTracking().Select(l => new LectureViewsCountDTO { Id = l.Id, Name = l.Name, ViewsCount = l.ViewsCount })];
        }

        public List<Lecture> GetCurrentLectures()
        {
            return [.. _context.Lectures.AsNoTracking().Where(l => l.Semester == currentSemester && l.UsedThisYear)];
        }

        public int GetViewsCount(string id)
        {
            return _context.Lectures.AsNoTracking().FirstOrDefault(l => l.Id == id)?.ViewsCount ?? 0;
        }

        public OperationResult<Lecture> GetLecture(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<Lecture>.Fail("Lecture ID cannot be null or empty.");

            var lecture = _context.Lectures.AsNoTracking().FirstOrDefault(l => l.Id == id);
            if (lecture == null)
                return OperationResult<Lecture>.Fail("Lecture not found.");

            return OperationResult<Lecture>.Ok(lecture, "Lecture retrieved successfully.");
        }

        public OperationResult<VideoDetailsDTO> GetVideoDetails(string id, int order)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<VideoDetailsDTO>.Fail("Lecture ID cannot be null or empty.");

            string videoId = _context.Lectures.AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => l.Videos[order].Id)
                .FirstOrDefault()!;

            VideoDetailsDTO? videoDetails = _context.Lectures.AsNoTracking()
                .Include(vd => vd.Videos)
                .Select(vd => new VideoDetailsDTO
                {
                    Id = videoId,
                    Name = vd.Name,
                    Description = vd.Description,
                    Path = vd.Videos[order].Path ?? string.Empty,
                    Unit = vd.Unit,
                    Order = order,
                    SheetPath = vd.SheetPath ?? string.Empty
                })
                .FirstOrDefault(vd => vd.Id == videoId && vd.Order == order);
            if (videoDetails == null)
                return OperationResult<VideoDetailsDTO>.Fail("Lecture not found.");

            return OperationResult<VideoDetailsDTO>.Ok(videoDetails, "Lecture retrieved successfully.");
        }

        private static bool IsValidGrade(int grade) => grade >= 1 && grade <= 6;

        public OperationResult<List<Lecture>> GetLecturesByGrade(int grade)
        {
            if (!IsValidGrade(grade))
                return OperationResult<List<Lecture>>.Fail("Grade must be between 1 and 6");
            var lectures = _context.Lectures.AsNoTracking().Where(l => l.Grade == grade).ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No current lectures found for grade {grade}.");
            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetCurrentLecturesByGrade(int grade)
        {
            if (!IsValidGrade(grade))
                return OperationResult<List<Lecture>>.Fail("Grade must be between 1 and 6");
            var lectures = _context.Lectures.AsNoTracking()
                .Where(l => /*l.Semester == ApplicationSettings.CurrentSemester && */l.UsedThisYear && l.Grade == grade)
                .ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No current lectures found for grade {grade}.");
            return OperationResult<List<Lecture>>.Ok(lectures, "Lectures retrieved successfully.");
        }

        public OperationResult<List<string>> GetUnits()
        {
            var units = _context.Lectures.AsNoTracking().Select(l => l.Unit).Distinct().ToList();
            if (units == null || units.Count == 0)
                return OperationResult<List<string>>.Fail("No units found.");
            return OperationResult<List<string>>.Ok(units, "Units retrieved successfully.");
        }

        public OperationResult<List<string>> GetCurrentUnits(int grade)
        {
            var units = _context.Lectures
                .Where(l => l.Grade == grade && l.Semester == currentSemester && l.UsedThisYear)
                .Select(l => l.Unit).Distinct().ToList();
            if (units == null || units.Count == 0)
                return OperationResult<List<string>>.Fail("No units found.");
            return OperationResult<List<string>>.Ok(units, "Units retrieved successfully.");
        }

        public OperationResult<List<Lecture>> GetLecturesByUnit(string unit)
        {
            var lectures = _context.Lectures.AsNoTracking().Where(l => l.Unit == unit).ToList();
            if (lectures == null || lectures.Count == 0)
                return OperationResult<List<Lecture>>.Fail($"No lectures found for unit '{unit}'.");
            return OperationResult<List<Lecture>>.Ok(lectures, $"Lectures for unit '{unit}' retrieved successfully.");
        }

        public OperationResult<Lecture> AddLecture(Lecture lecture)
        {
            if (lecture == null)
                return OperationResult<Lecture>.Fail("Lecture cannot be null.");

            _context.Lectures.Add(lecture);
            _context.SaveChanges();

            return OperationResult<Lecture>.Ok(lecture, "Lecture inserted successfully.");
        }

        public async Task<OperationResult<int>> UpdateLectureAsync(string id, Lecture lecture)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<int>.Fail("Lecture ID cannot be null or empty.");

            if (lecture == null)
                return OperationResult<int>.Fail("Lecture cannot be null.");

            var existingLecture = _context.Lectures.FirstOrDefault(l => l.Id == id);
            if (existingLecture == null)
                return OperationResult<int>.Fail("Lecture not found.");

            _context.Entry(existingLecture).CurrentValues.SetValues(lecture);
            try
            {
                await _context.SaveChangesAsync();
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

            var lecture = _context.Lectures.FirstOrDefault(l => l.Id == id);
            if (lecture == null)
                return OperationResult<bool>.Fail("Lecture not found.");

            try
            {
                _context.Lectures.Remove(lecture);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true, "Lecture deleted successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Error deleting lecture: {ex.Message}");
            }
        }
    }
}
