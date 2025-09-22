using Microsoft.EntityFrameworkCore;
using TadrousManassa.Areas.Student.Models;
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
        private readonly IStudentChoiceRepository _studentChoiceRepository;
        private readonly IQuizService _quizService;

        public StudentQuizService(
            IStudentQuizRepository studentQuizRepository,
            ISubmissionRepository submissionRepository,
            IQuizRepository quizRepository,
            IStudentChoiceRepository studentChoiceRepository,
            IQuizService quizService)
        {
            _studentQuizRepository = studentQuizRepository;
            _submissionRepository = submissionRepository;
            _quizRepository = quizRepository;
            _studentChoiceRepository = studentChoiceRepository;
            _quizService = quizService;
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
                if (attempts == -1)
                {
                    var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                    return quiz is not null
                        ? OperationResult<int>.Ok(quiz.TotalNumOfAttempts, "No attempts recorded, returning total attempts")
                        : OperationResult<int>.Fail("Quiz not found");
                }
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

        public async Task<OperationResult<int>> DecreaseNumOfRemainingAttemptsAsync(string studentId, string quizId)
        {
            var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId);

            if (studentQuiz is null)
                return OperationResult<int>.Fail("This user has not bought this lecture");

            if (studentQuiz.NumOfRemainingAttempts <= 0)
                return OperationResult<int>.Fail("No attempts remaining");

            try
            {
                studentQuiz.NumOfRemainingAttempts--;
                await _studentQuizRepository.SaveChangesAsync();
                return OperationResult<int>.Ok(studentQuiz.NumOfRemainingAttempts, "Attempt decreased successfully");
            }
            catch (Exception)
            {
                return OperationResult<int>.Fail("Something happened while registering your attempt, please try again");
            }
        }

        public async Task<OperationResult<SavingSubmissionDTO>> SaveSubmissionAsync(
            string studentId,
            string quizId,
            DateTime quizStartTime,
            Dictionary<string, string> answers)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(quizId))
            {
                return OperationResult<SavingSubmissionDTO>.Fail("Student ID and Quiz ID are required");
            }
            try
            {
                bool isFirstSubmission = false;
                var studentQuiz = await _studentQuizRepository.GetStudentQuizAsync(studentId, quizId, true);
                // TODO: Should I check if there any submissions before or these two conditions enough?
                if (studentQuiz is null || studentQuiz.BestScore is null)
                {
                    isFirstSubmission = true;
                }
                var quiz = await _quizRepository.GetQuizByIdAsync(quizId);
                if (quiz is null)
                {
                    return OperationResult<SavingSubmissionDTO>.Fail("Quiz not found");
                }
                StudentQuizScoresDTO scores;
                if (studentQuiz is null)
                {
                    
                    studentQuiz = new StudentQuiz
                    {
                        Id = Guid.NewGuid().ToString(),
                        StudentId = studentId,
                        QuizId = quizId,
                        Quiz = quiz,
                        Submissions = new List<Submission>(),
                        NumOfRemainingAttempts = quiz.TotalNumOfAttempts
                    };

                    scores = CalculateStudentAndTotalScoresAsync(quiz, answers);
                    studentQuiz.BestScore = scores.Score;
                    studentQuiz.IsSuccess = (scores.Score / scores.TotalScore) >= 0.5f;

                    await _studentQuizRepository.AddStudentQuizAsync(studentQuiz);
                }
                else
                {
                    scores = CalculateStudentAndTotalScoresAsync(quiz, answers);
                    studentQuiz.BestScore = Math.Max(studentQuiz.BestScore ?? 0, scores.Score);
                    studentQuiz.IsSuccess = studentQuiz?.IsSuccess == true
                        || (scores.Score / scores.TotalScore) >= 0.5f;
                }

                // new StudentQuiz creation logic would go here if necessary
                var submission = new Submission
                    {
                        Id = Guid.NewGuid().ToString(),
                        StartTime = quizStartTime,
                        StudentQuizId = studentQuiz.Id,
                        StudentQuiz = studentQuiz,
                        SubmissionTime = DateTime.Now,
                        Score = scores.Score,
                        OrderOfSubmission = isFirstSubmission || studentQuiz.Submissions.Count == 0 ? 1 : (studentQuiz.Submissions?
                                                                        .Where(s => s.StudentQuizId == studentQuiz.Id)
                                                                        .Max(s => s.OrderOfSubmission) + 1) ?? 1
                };
                studentQuiz.Submissions.Add(submission);
                //await _submissionRepository.AddSubmissionAsync(submission);
                await _studentQuizRepository.SaveChangesAsync();

                return OperationResult<SavingSubmissionDTO>.Ok(new SavingSubmissionDTO { Score = scores.Score, SubmissionId = submission.Id }, "Submission saved successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<SavingSubmissionDTO>.Fail($"Error saving quiz submission: {ex.Message}");
            }
        }

        private StudentQuizScoresDTO CalculateStudentAndTotalScoresAsync(Quiz quiz, Dictionary<string, string> answers)
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

            return new StudentQuizScoresDTO{ Score = correctAnswers, TotalScore = totalQuestions };
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

            var result = await _quizService.GetQuizResultAsync(studentId, quizId, remainingAttempts, answers);
            return OperationResult<QuizResultDTO?>.Ok(result);
        }
    }
}
