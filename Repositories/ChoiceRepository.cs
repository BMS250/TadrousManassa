using Microsoft.EntityFrameworkCore;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class ChoiceRepository(ApplicationDbContext context) : IChoiceRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Choice?> GetChoiceByIdAsync(string id)
        {
            return await _context.Choices.FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
