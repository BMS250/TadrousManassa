using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Repositories;
using TadrousManassa.Repositories.IRepositories;
using TadrousManassa.Services.IServices;

namespace TadrousManassa.Services
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly IStudentQuizRepository _studentQuizRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IQuizRepository _quizRepository;

        public StudentQuizService(
            IStudentQuizRepository studentQuizRepository,
            ISubmissionRepository submissionRepository,
            IQuizRepository quizRepository)
        {
            _studentQuizRepository = studentQuizRepository;
            _submissionRepository = submissionRepository;
            _quizRepository = quizRepository;
        }

        public async Task<OperationResult<List<Quiz>>> GetFullQuizzesByStudentIdAsync(string studentId)
        {
            try
            {
                var quizzes = await _studentQuizRepository.GetFullQuizzesByStudentIdAsync(studentId);
                return OperationResult<List<Quiz>>.Ok(quizzes);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Quiz>>.Fail($"Error fetching quizzes: {ex.Message}");
            }
        }

        public async Task<OperationResult<int>> GetRemainingAttemptsByQuizIdAsync(string studentId, string quizId)
        {
            try
            {
                var attempts = await _studentQuizRepository.GetRemainingAttemptsByQuizIdAsync(studentId, quizId);
                return OperationResult<int>.Ok(attempts);
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Error fetching attempts: {ex.Message}");
            }
        }

        public async Task<OperationResult<int>> GetRemainingAttemptsByVideoIdAsync(string studentId, string videoId)
        {
            try
            {
                var attempts = await _studentQuizRepository.GetRemainingAttemptsByVideoIdAsync(studentId, videoId);
                return OperationResult<int>.Ok(attempts);
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Error fetching attempts: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> IsQuizSolved(string studentId, string videoId)
        {
            try
            {
                var solved = await _studentQuizRepository.IsQuizSolved(studentId, videoId);
                return OperationResult<bool>.Ok(solved);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Error checking if quiz solved: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId)
        {
            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId);

            if (studentQuiz is null)
                return OperationResult<bool>.Fail("This user has not bought this lecture");

            if (studentQuiz.NumOfRemainingAttempts <= 0)
                return OperationResult<bool>.Fail("No attempts remaining");

            try
            {
                studentQuiz.NumOfRemainingAttempts--;
                await _studentQuizRepository.SaveChangesAsync();
                return OperationResult<bool>.Ok(true, "Attempt decreased successfully");
            }
            catch (Exception)
            {
                return OperationResult<bool>.Fail("Something happened while registering your attempt, please try again");
            }
        }

        public async Task<OperationResult<int>> SaveQuizSubmissionAsync(
            string studentId,
            string quizId,
            DateTime quizStartTime,
            Dictionary<string, string> answers)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(quizId))
            {
                return OperationResult<int>.Fail("Student ID and Quiz ID are required");
            }
            try
            {
                var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId, true);
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz is null)
                {
                    return OperationResult<int>.Fail("Quiz not found");
                }
                Tuple<float, float> scores;
                if (studentQuiz is null)
                {
                    
                    studentQuiz = new StudentQuiz
                    {
                        Id = Guid.NewGuid().ToString(),
                        StudentId = studentId,
                        QuizId = quizId,
                        Quiz = quiz,
                        Submissions = new List<QuizSubmission>(),
                        NumOfRemainingAttempts = 2,
                    };

                    scores = CalculateStudentAndTotalScoresAsync(quiz, answers);
                    studentQuiz.BestScore = scores.Item1;
                    studentQuiz.IsSuccess = (scores.Item1 / scores.Item2) >= 0.5f;

                    await _studentQuizRepository.AddStudentQuizAsync(studentQuiz);
                }
                else
                {
                    scores = CalculateStudentAndTotalScoresAsync(quiz, answers);
                    studentQuiz.BestScore = Math.Max(studentQuiz.BestScore ?? 0, scores.Item1);
                    studentQuiz.IsSuccess = (scores.Item1 / scores.Item2) >= 0.5f;
                }

                    // new StudentQuiz creation logic would go here if necessary
                    var submission = new QuizSubmission
                    {
                        Id = Guid.NewGuid().ToString(),
                        StartTime = quizStartTime,
                        StudentQuizId = studentQuiz.Id,
                        StudentQuiz = studentQuiz,
                        SubmissionTime = DateTime.Now,
                        Score = scores.Item1
                    };
                studentQuiz.Submissions.Add(submission);
                //await _submissionRepository.AddSubmissionAsync(submission);
                await _studentQuizRepository.SaveChangesAsync();

                return OperationResult<int>.Ok(studentQuiz.NumOfRemainingAttempts, "Submission saved successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Error saving quiz submission: {ex.Message}");
            }
        }

        private Tuple<float, float> CalculateStudentAndTotalScoresAsync(Quiz quiz, Dictionary<string, string> answers)
        {
            float totalQuestions = 0;
            float correctAnswers = 0;

            foreach (var question in quiz.Questions)
            {
                totalQuestions += question.Score;

                if (answers.TryGetValue(question.Id, out var selectedChoiceId))
                {
                    if (selectedChoiceId == question.AnswerId)
                    {
                        correctAnswers += question.Score;
                    }
                }
            }

            return new(correctAnswers, totalQuestions);
        }
    }
}
