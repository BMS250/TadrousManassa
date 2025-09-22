using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class QuizRepository(ApplicationDbContext context) : IQuizRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<bool> IsQuizIdExists(string id)
        {
            return await _context.Quizzes.AnyAsync(q => q.Id == id);
        }

        public async Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<OperationResult<string>> GetLectureIdByQuizId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<string>.Fail("Quiz ID cannot be null or empty.");
            var lectureId = await _context.Quizzes
                .AsNoTracking()
                .Where(q => q.Id == id)
                .Select(q => q.LectureId)
                .FirstOrDefaultAsync();
            if (lectureId == null)
                return OperationResult<string>.Fail("Quiz not found.");
            return OperationResult<string>.Ok(lectureId, "Lecture ID retrieved successfully.");
        }

        public Task<QuizDetailsDTO?> GetQuizDetailsAsync(string id)
        {
            return _context.Quizzes
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name ?? "",
                    Description = q.Description ?? "",
                    TimeHours = q.TimeHours,
                    TimeMinutes = q.TimeMinutes
                })
                .FirstOrDefaultAsync(q => q.Id == id);
        }
        
        public Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId)
        {
            return _context.Quizzes
                .Where(q => q.LectureId == lectureId)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .ToListAsync();
        }

        public Task<QuizResultDTO?> GetQuizResultAsync(string studentId, string quizId, int remainingAttempts, Dictionary<string, string> answers)
        {
            return _context.Quizzes
                .Where(q => q.Id == quizId)
                .Select(q => new QuizResultDTO
                {
                    QuizId = q.Id,
                    TotalScore = q.TotalScore,
                    QuizTitle = q.Name,
                    Questions = q.Questions.Select(ques => new QuestionResultDTO
                    {
                        QuestionId = ques.Id,
                        QuestionText = ques.Text,
                        QuestionImage = ques.Image,
                        Score = ques.Score,
                        
                        Choices = ques.Choices
                        .Select(c => new ChoiceResultDTO
                        {
                            ChoiceId = c.Id,
                            ChoiceText = c.Text,
                            ChoiceImage = c.Image
                        }).ToList(),

                        //SummarizedStudentChoice = ques.Choices
                        //    .SelectMany(c => c.StudentChoices)
                        //    .Where(sc => sc.StudentId == studentId)
                        //    .Select(sc => new SummarizedStudentChoice
                        //    {
                        //        ChoiceId = sc.ChoiceId,
                        //        IsCorrect = sc.IsCorrect
                        //    })
                        //    .FirstOrDefault(),
                        // Use the answers dictionary to get the selected choice for this question
                        SelectedChoiceId = answers.ContainsKey(ques.Id) ? answers[ques.Id] : null,

                        // Check if the selected answer is correct by comparing with the correct answer
                        IsCorrect = answers.ContainsKey(ques.Id) && answers[ques.Id] == ques.AnswerId,

                        CorrectAnswerId = remainingAttempts == 0 ? ques.AnswerId : null,
                        CorrectAnswerText = remainingAttempts == 0 ? ques.Answer.Text : null
                    }).ToList(),
                    RemainingAttempts = remainingAttempts
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalNumOfAttemptsAsync(string id)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id);
            return quiz is null ? throw new InvalidOperationException("Quiz not found.") : quiz.TotalNumOfAttempts;
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(string id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz != null)
            {
                foreach (var question in quiz.Questions)
                {
                    _context.Choices.RemoveRange(question.Choices);
                }

                _context.Questions.RemoveRange(quiz.Questions);
                _context.Quizzes.Remove(quiz);

                await _context.SaveChangesAsync();
            }
        }

    }
}
