using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public class OfflineQuizRepository : IOfflineQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public OfflineQuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddOfflineQuiz(OfflineQuiz offlineQuiz)
        {
            await _context.OfflineQuizzes.AddAsync(offlineQuiz);
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
