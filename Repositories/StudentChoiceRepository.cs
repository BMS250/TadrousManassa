using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class StudentChoiceRepository : IStudentChoiceRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentChoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<StudentChoice?> GetStudentChoiceByIdAsync(string id)
        {
            return _context.StudentChoices
                .Include(sc => sc.Student)
                .Include(sc => sc.Choice)
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public Task<Dictionary<string, bool>> GetCorrectnessAsync(string studentId, string quizId)
        {
            return _context.StudentChoices
                .Include(sc => sc.Choice)
                .ThenInclude(c => c.Question)
                .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
                .Select(sc => new
                {
                    sc.Choice.QuestionId,
                    sc.IsCorrect
                })
                .ToDictionaryAsync(x => x.QuestionId!, x => x.IsCorrect);
        }

        public Task<Dictionary<string, KeyValuePair<bool?, string>>> GetCorrectnessAndRightAnswersAsync(string studentId, string quizId)
        {
            return _context.StudentChoices
                .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
                .Select(sc => new
                {
                    sc.Choice.QuestionId,
                    sc.IsCorrect,
                    RightAnswerId = sc.Choice.Question.AnswerId
                })
                .ToDictionaryAsync(x => x.QuestionId!, x => new KeyValuePair<bool?, string> (x.IsCorrect, x.RightAnswerId));
        }

        public Task<Dictionary<string, string>> GetAnswersBySubmissionIdAsync(string lastSubmissionId)
        {
            return _context.StudentChoices
                .Where(sc => sc.SubmissionId == lastSubmissionId)
                .Select(sc => new
                {
                    sc.Choice.QuestionId,
                    sc.ChoiceId
                })
                .ToDictionaryAsync(x => x.QuestionId!, x => x.ChoiceId);
        }

        public async Task AddStudentChoicesAsync(string studentId, string quizId, string submissionId, List<string> answerIds)
        {
            // Fetch all relevant choices + their questions in one roundtrip
            var choices = await _context.Choices
                .Where(c => answerIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Question.AnswerId })
                .ToListAsync();

            var studentChoices = choices.Select(c => new StudentChoice
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = studentId,
                ChoiceId = c.Id,
                IsCorrect = c.AnswerId == c.Id,
                SubmissionId = submissionId
            }).ToList();

            await _context.StudentChoices.AddRangeAsync(studentChoices);
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
