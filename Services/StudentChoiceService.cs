using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class StudentChoiceService : IStudentChoiceService
    {
        private readonly IStudentChoiceRepository _studentChoiceRepository;
        public StudentChoiceService(IStudentChoiceRepository studentChoiceRepository)
        {
            _studentChoiceRepository = studentChoiceRepository;
        }

        public Task<StudentChoice?> GetStudentChoiceByIdAsync(string id)
        {
            return _studentChoiceRepository.GetStudentChoiceByIdAsync(id);
        }

        public Task<Dictionary<string, bool>> GetCorrectnessAsync(string studentId, string quizId)
        {
            return _studentChoiceRepository.GetCorrectnessAsync(studentId, quizId);
        }

        public Task<Dictionary<string, KeyValuePair<bool?, string>>> GetCorrectnessAndRightAnswersAsync(string studentId, string quizId)
        {
            return _studentChoiceRepository.GetCorrectnessAndRightAnswersAsync(studentId, quizId);
        }

        public async Task AddStudentChoicesAsync(string studentId, string quizId, string submissionId, List<string> answerIds)
        {
            await _studentChoiceRepository.AddStudentChoicesAsync(studentId, quizId, submissionId, answerIds);
            await _studentChoiceRepository.SaveChangesAsync();
        }

    }
}
