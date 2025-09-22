using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IStudentQuizRepository _studentQuizRepository;
        public SubmissionService(ISubmissionRepository submissionRepository, IStudentQuizRepository studentQuizRepository)
        {
            _submissionRepository = submissionRepository;
            _studentQuizRepository = studentQuizRepository;
        }

        public Task AddSubmissionAsync(Submission quizSubmission)
        {
            return _submissionRepository.AddSubmissionAsync(quizSubmission);
        }
        public async Task<OperationResult<int>> GetMaxSubmissionOrder(string studentId, string quizId)
        {
            var studentQuizId = await _studentQuizRepository.GetStudentQuizId(studentId, quizId);
            if (studentQuizId == null)
            {
                return OperationResult<int>.Ok(0, "Student has not purchased this quiz yet.");
            }

            var maxOrder = await _submissionRepository.GetMaxSubmissionOrder(studentQuizId);
            if (maxOrder == null)
            {
                // If no submissions found, return success with null string
                return OperationResult<int>.Ok(0, "No submissions found for this student quiz.");
            }

            // If maxOrder is null, return success with null string
            return OperationResult<int>.Ok((int)maxOrder);
        }

        public Task<string?> GetIdOfLastSubmissionOrder(string studentQuizId, int maxOrder)
        {
            return _submissionRepository.GetIdOfLastSubmissionOrder(studentQuizId, maxOrder);
        }
    }
}
