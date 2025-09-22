using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class QuestionRepository(ApplicationDbContext context) : IQuestionRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Question?> GetQuestionByIdAsync(string questionId)
        {
            return await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<List<Question>> GetQuestionsByLectureIdAsync(string lectureId)
        {
            // Revise Loading
            return await _context.Questions
                .Where(q => q.Quiz.LectureId == lectureId)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsByQuizIdAsync(string quizId)
        {
            return await _context.Questions.Where(q => q.QuizId == quizId).ToListAsync();
        }

        public async Task AddQuestionToQuizAsync(Quiz quiz, Question question)
        {
            _context.Questions.Add(question);
            _context.Quizzes.Add(quiz);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Add OperationResult to handle exceptions
                throw new Exception("Error adding question to quiz. Please try again later.");
            }

        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Add OperationResult to handle exceptions
                throw new Exception("Error updating question. Please try again later.");
            }
        }

        public async Task DeleteQuestionAsync(string questionId)
        {
            var question = await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question != null)
            {
                _context.Choices.RemoveRange(question.Choices);
                _context.Questions.Remove(question);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // Add OperationResult to handle exceptions
                    throw new Exception("Error updating question. Please try again later.");
                }
            }
        }
    }
}
