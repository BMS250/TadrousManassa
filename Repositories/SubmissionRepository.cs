using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Models.ViewModels;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubmissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSubmissionAsync(QuizSubmission quizSubmission)
        {
           await _context.QuizSubmissions.AddAsync(quizSubmission);
        }

    }
}
