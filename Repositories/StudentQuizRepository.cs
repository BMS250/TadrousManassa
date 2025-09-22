using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class StudentQuizRepository : IStudentQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentQuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<string?> GetStudentQuizId(string studentId, string quizId)
        {
            return _context.StudentQuizzes
                .AsNoTracking()
                .Where(sq => sq.StudentId == studentId && sq.QuizId == quizId)
                .Select(sq => sq.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId)
        {
            return await _context.StudentQuizzes
                .Where(sq => sq.StudentId == studentId)
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Choices)
                .Select(sq => sq.Quiz)
                .ToListAsync();
        }

        public async Task SaveSubmissionAsync(string studentId, string quizId, Dictionary<string, string> answers)
        {
            // find student quiz entry
            var studentQuiz = await _context.StudentQuizzes
                .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

            if (studentQuiz == null)
            {
                // TODO: remove thiis exception and handle it in service
                throw new InvalidOperationException("Student has not purchased this quiz.");
            }

            // create submission entity
            //var submission = new Submission
            //{
            //    StudentId = studentId,
            //    QuizId = quizId,
            //    SubmittedAt = DateTime.Now,
            //    AnswersJson = System.Text.Json.JsonSerializer.Serialize(answers)
            //};

            //_context.Submissions.Add(submission);

            // decrease attempt count YOU DID IT IN SERVICE
            ////studentQuiz.NumOfRemainingAttempts--;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .AsNoTracking()
                .Where(sq => sq.StudentId == studentId && sq.QuizId == quizId)
                .FirstOrDefaultAsync();

            if (studentQuiz == null)
                return -1; // Default attempts if no record exists
            return studentQuiz.NumOfRemainingAttempts;
        }

        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId)
        {
            var quizId = _context.Quizzes
                .AsNoTracking()
                .Where(q => q.VideoId == videoId)
                .Select(q => q.Id)
                .FirstOrDefault();
            return GetRemainingAttemptsByQuizIdAsync(studentId, quizId!);
        }

        public async Task<bool> IsQuizSolved(string studentId, string videoId)
        {
            var quizId = await _context.Quizzes
                .AsNoTracking()
                .Where(q => q.VideoId == videoId)
                .Select(q => q.Id)
                .FirstOrDefaultAsync();
            return await _context.StudentQuizzes.AnyAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

        public async Task<StudentQuiz?> GetStudentQuizAsync(string studentId, string quizId, bool includeQuiz = false)
        {
            IQueryable<StudentQuiz> query = _context.StudentQuizzes;
            if (includeQuiz)
            {
                query = query
                    .Include(sq => sq.Quiz)
                    .Include(sq => sq.Submissions);
            }

            return await query.FirstOrDefaultAsync(
                sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

        public async Task<StudentQuizScoresDTO?> GetStudentQuizScoresAsync(string studentId, string quizId)
        {
            return await _context.StudentQuizzes
                .AsNoTracking()
                .Include(sq => sq.Quiz)
                .Where(sq => sq.StudentId == studentId && sq.QuizId == quizId)
                .Select(sq => new StudentQuizScoresDTO { Score = sq.BestScore ?? 0, TotalScore = sq.Quiz.TotalScore })
                .FirstOrDefaultAsync();
        }

        public async Task<float> GetBestScoreAsync(string studentId, string quizId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

            return studentQuiz?.BestScore ?? 0;
        }

        public async Task AddStudentQuizAsync(StudentQuiz studentQuiz)
        {
            await _context.StudentQuizzes.AddAsync(studentQuiz);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
