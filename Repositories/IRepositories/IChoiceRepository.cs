using System.Collections.Generic;
using System.Threading.Tasks;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IChoiceRepository
    {
        public Task<Choice?> GetChoiceByIdAsync(string id);
    }
}
