using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
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

        public async Task SaveQuizSubmissionAsync(string studentId, string quizId, Dictionary<string, string> answers)
        {
            // find student quiz entry
            var studentQuiz = await _context.StudentQuizzes
                .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

            if (studentQuiz == null)
            {
                throw new InvalidOperationException("Student has not purchased this quiz.");
            }

            // create submission entity
            //var submission = new QuizSubmission
            //{
            //    StudentId = studentId,
            //    QuizId = quizId,
            //    SubmittedAt = DateTime.Now,
            //    AnswersJson = System.Text.Json.JsonSerializer.Serialize(answers)
            //};

            //_context.QuizSubmissions.Add(submission);

            // decrease attempt count YOU DID IT IN SERVICE
            ////studentQuiz.NumOfRemainingAttempts--;

            await _context.SaveChangesAsync();
        }

        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId)
        {
            return _context.StudentQuizzes
                .AsNoTracking()
                .Where(sq => sq.StudentId == studentId && sq.QuizId == quizId)
                .Select(sq => sq.NumOfRemainingAttempts)
                .FirstOrDefaultAsync();
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

        public async Task<StudentQuiz?> GetStudentQuizAsync(string studentId, string quizId)
        {
            return await _context.StudentQuizzes
                .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);
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
