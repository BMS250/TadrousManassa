using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly StudentQuizRepository _studentQuizRepository;
        public StudentQuizService(StudentQuizRepository studentQuizRepository)
        {
            _studentQuizRepository = studentQuizRepository;
        }

        public Task<List<Quiz>> GetFullQuizzesByStudentIdAsync(string studentId)
        {
            return _studentQuizRepository.GetFullQuizzesByStudentIdAsync(studentId);
        }

        public bool IsQuizTaken(string studentId, string quizId)
        {
            return _studentQuizRepository.IsQuizTaken(studentId, quizId);
        }
    }
}
