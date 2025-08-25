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

        public bool IsQuizSolved(string studentId, string videoId)
        {
            return _studentQuizRepository.IsQuizSolved(studentId, videoId);
        }
    }
}
