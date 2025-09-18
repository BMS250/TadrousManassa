using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
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

        public async Task<int> SaveQuizSubmissionAsync(string studentId, string quizId, DateTime quizStartTime, Dictionary<string, string> answers)
        {
            // check if there is a student quiz entry
            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId, true);
            // TODO make sure the quiz exists
            var scores = await CalculateStudentAndTotalScoresAsync(studentQuiz.Quiz, answers);
            // if not, create a new one
            if (studentQuiz is null)
            {
                studentQuiz = new StudentQuiz
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Submissions = new List<QuizSubmission>(),
                    NumOfRemainingAttempts = 2,
                    BestScore = scores.Item1,
                    IsSuccess = (scores.Item1 / scores.Item2) >= 0.5f
                };

                await _studentQuizRepository.AddStudentQuizAsync(studentQuiz);
                await _studentQuizRepository.SaveChangesAsync();
            }
            var submission = new QuizSubmission
            {
                StartTime = quizStartTime,
                StudentQuizId = studentQuiz.Id,
                StudentQuiz = studentQuiz,
                SubmissionTime = DateTime.Now,
                Score = scores.Item1
            };
            await _studentQuizRepository.SaveChangesAsync();
            return studentQuiz.NumOfRemainingAttempts;
        }

        async Task<Tuple<float, float>> CalculateStudentAndTotalScoresAsync(Quiz quiz, Dictionary<string, string> answers)
        {
            float totalQuestions = 0;
            float correctAnswers = 0;
            foreach (var question in quiz.Questions)
            {
                totalQuestions += question.Score;
                if (answers.TryGetValue(question.Id, out var selectedChoiceId))
                {
                    var selectedChoice = question.Choices.FirstOrDefault(c => c.Id == selectedChoiceId);
                    if (selectedChoice != null && selectedChoiceId == question.AnswerId)
                    {
                        correctAnswers += question.Score;
                    }
                }
            }
            return new(correctAnswers, totalQuestions);
        }
    }
}