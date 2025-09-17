using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly IStudentQuizRepository _studentQuizRepository;
        public StudentQuizService(IStudentQuizRepository studentQuizRepository)
        {
            _studentQuizRepository = studentQuizRepository;
        }

        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId)
        {
            return _studentQuizRepository.GetFullQuizzesByStudentIdAsync(studentId);
        }

        public Task<int> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId)
        {
            return _studentQuizRepository.GetRemainingAttemptsByQuizIdAsync(studentId, quizId);
        }
        
        public Task<int> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId)
        {
            return _studentQuizRepository.GetRemainingAttemptsByVideoIdAsync(studentId, videoId);
        }

        public Task<bool> IsQuizSolved(string studentId, string videoId)
        {
            return _studentQuizRepository.IsQuizSolved(studentId, videoId);
        }

        public async Task<OperationResult<bool>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId)
        {
            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId);

            if (studentQuiz is null)
            {
                return OperationResult<bool>.Fail("This user has not bought this lecture");
            }

            if (studentQuiz.NumOfRemainingAttempts <= 0)
            {
                return OperationResult<bool>.Fail("No attempts remaining");
            }

            try
            {
                studentQuiz.NumOfRemainingAttempts--;
                await _studentQuizRepository.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Fail("Something happened while registering your attempt, please try again");
            }
        }

        public async Task<DateTime> GetOrCreateQuizStartTimeAsync(string studentId, string quizId)
        {
            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId);

            if (studentQuiz is null)
            {
                studentQuiz = new StudentQuiz
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    StartTime = DateTime.Now,
                    NumOfRemainingAttempts = 2
                };

                await _studentQuizRepository.AddStudentQuizAsync(studentQuiz);
                await _studentQuizRepository.SaveChangesAsync();
            }
            else if (studentQuiz.StartTime == null)
            {
                studentQuiz.StartTime = DateTime.Now;
                await _studentQuizRepository.SaveChangesAsync();
            }

            return studentQuiz.StartTime;
        }
    }
}
