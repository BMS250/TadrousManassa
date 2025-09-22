using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IStudentQuizService _studentQuizService;
        private readonly IVideoService _videoService;
        private readonly IStudentQuizRepository _studentQuizRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IStudentChoiceRepository _studentChoiceRepository;

        public QuizService(IQuizRepository quizRepository, IStudentQuizService studentQuizService, IVideoService videoService, IStudentQuizRepository studentQuizRepository, ISubmissionRepository submissionRepository, IStudentChoiceRepository studentChoiceRepository)
        {
            _quizRepository = quizRepository;
            _studentQuizService = studentQuizService;
            _videoService = videoService;
            _studentQuizRepository = studentQuizRepository;
            _submissionRepository = submissionRepository;
            _studentChoiceRepository = studentChoiceRepository;
        }

        public Task<Quiz?> GetQuizByIdAsync(string id)
        {
            return _quizRepository.GetQuizByIdAsync(id);
        }

        public async Task<OperationResult<QuizDetailsDTO?>> GetQuizDetailsAsync(string id)
        {
            var quizDetails = await _quizRepository.GetQuizDetailsAsync(id);

            if (quizDetails == null)
            {
                return OperationResult<QuizDetailsDTO?>.Fail("Quiz not found");
            }

            return OperationResult<QuizDetailsDTO?>.Ok(quizDetails);
        }


        public Task<OperationResult<string>> GetLectureIdByQuizId(string id)
        {
            return _quizRepository.GetLectureIdByQuizId(id);
        }

        public Task<List<Quiz>> GetQuizzesByLectureIdAsync(string lectureId)
        {
            return _quizRepository.GetQuizzesByLectureIdAsync(lectureId);
        }

        public async Task<QuizResultDTO?> GetQuizResultAsync(string studentId, string quizId, int remainingAttempts, Dictionary<string, string> answers)
        {
            var bestScore = await _studentQuizService.GetBestScoreAsync(studentId, quizId);
            var nextVideoExists = await _videoService.CheckAndGetNextVideoOrderByQuizIdAsync(quizId);
            var quizResult = await _quizRepository.GetQuizResultAsync(studentId, quizId, remainingAttempts, answers);

            if (quizResult != null)
            {
                quizResult.BestScore = bestScore;
                if (nextVideoExists.Success)
                {
                    if (nextVideoExists.Data == -1)
                    {
                        quizResult.IsNextVideoExists = false;
                    }
                    else
                    {
                        quizResult.IsNextVideoExists = true;
                        quizResult.NextVideoOrder = nextVideoExists.Data;
                    }
                }
            }

            return quizResult;
        }

        public async Task<OperationResult<QuizResultDTO?>> GetQuizResultOfLastSubmissionAsync(
            string studentId, string quizId, int remainingAttempts)
        {
            var studentQuizId = await _studentQuizRepository.GetStudentQuizId(studentId, quizId);
            if (studentQuizId is null)
            {
                return OperationResult<QuizResultDTO?>.Fail("Student has not purchased this quiz.");
            }

            int? maxOrder = await _submissionRepository.GetMaxSubmissionOrder(studentQuizId);
            if (maxOrder is null)
            {
                return OperationResult<QuizResultDTO?>.Fail("No submissions found for this student quiz.");
            }

            var lastSubmissionId = await _submissionRepository.GetIdOfLastSubmissionOrder(studentQuizId, (int)maxOrder);
            if (lastSubmissionId is null)
            {
                return OperationResult<QuizResultDTO?>.Fail("Could not find last submission.");
            }

            var answers = await _studentChoiceRepository.GetAnswersBySubmissionIdAsync(lastSubmissionId);
            if (answers is null || answers.Count == 0)
            {
                return OperationResult<QuizResultDTO?>.Fail("No answers found for the last submission.");
            }

            var result = await GetQuizResultAsync(studentId, quizId, remainingAttempts, answers);
            if (result is null)
            {
                return OperationResult<QuizResultDTO?>.Fail("Could not compute quiz result.");
            }
            return OperationResult<QuizResultDTO?>.Ok(result);
        }

        public Task CreateQuizAsync(Quiz quiz)
        {
            return _quizRepository.CreateQuizAsync(quiz);
        }

        public Task UpdateQuizAsync(Quiz quiz)
        {
            return _quizRepository.UpdateQuizAsync(quiz);
        }

        public Task DeleteQuizAsync(string id)
        {
            return _quizRepository.DeleteQuizAsync(id);
        }

    }
}
