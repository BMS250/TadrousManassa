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

        public Task<Dictionary<string, bool?>> GetCorrectnessAsync(string studentId, string quizId)
        {
            return _context.StudentChoices
                .Include(sc => sc.Choice)
                .ThenInclude(c => c.Question)
                .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
                .Select(sc => new
                {
                    QuestionId = sc.Choice.QuestionId,
                    IsCorrect = sc.IsCorrect
                })
                .ToDictionaryAsync(x => x.QuestionId, x => x.IsCorrect);
        }

        public Task<Dictionary<string, KeyValuePair<bool?, string>>> GetCorrectnessAndRightAnswersAsync(string studentId, string quizId)
        {
            return _context.StudentChoices
                .Include(sc => sc.Choice)
                .ThenInclude(c => c.Question)
                .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
                .Select(sc => new
                {
                    sc.Choice.QuestionId,
                    sc.IsCorrect,
                    RightAnswerId = sc.Choice.Question.AnswerId
                })
                .ToDictionaryAsync(x => x.QuestionId, x => new KeyValuePair<bool?, string> (x.IsCorrect, x.RightAnswerId));
        }

        public Task<Dictionary<string, KeyValuePair<string, bool?>>> GetCorrectnessAsyncX(string studentId, string quizId)
        {
            //return _context.StudentChoices
            //    .Include(sc => sc.Choice)
            //    .ThenInclude(c => c.Question)
            //    .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
            //    .Select(sc => new
            //    {
            //        Question = sc.Choice.Question.Text,
            //        QuestionImage = sc.Choice.Question.Image,
            //        Choice = sc.Choice.Text,
            //        ChoiceImage = sc.Choice.Image,
            //        sc.IsCorrect
            //    })
            //    .ToDictionaryAsync(x => x.QuestionId, x => x.IsCorrect);
            return null!;
        }

        public Task<Dictionary<string, KeyValuePair<KeyValuePair<string, bool?>, string>>> GetCorrectnessAndRightAnswersAsyncX(string studentId, string quizId)
        {
            //return _context.StudentChoices
            //    .Include(sc => sc.Choice)
            //    .ThenInclude(c => c.Question)
            //    .Where(sc => sc.StudentId == studentId && sc.Choice.Question.QuizId == quizId)
            //    .Select(sc => new
            //    {
            //        sc.Choice.QuestionId,
            //        sc.IsCorrect,
            //        RightAnswerId = sc.Choice.Question.AnswerId
            //    })
            //    .ToDictionaryAsync(x => x.QuestionId, x => new KeyValuePair<bool?, string> (x.IsCorrect, x.RightAnswerId));
            return null!;
        }

        public async Task AddStudentChoiceAsync(string studentId, string quizId, List<string> questionIdsWithAnswerIds)
        {
            await _context.StudentChoices.AddRangeAsync(
                questionIdsWithAnswerIds.Select(qa => new StudentChoice
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    ChoiceId = qa
                })
            );
            await _context.SaveChangesAsync();
        }
    }
}
