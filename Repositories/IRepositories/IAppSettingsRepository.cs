using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories.IRepositories
{
    public interface IAppSettingsRepository
    {
        public ApplicationSettings GetCurrentData();
        public OperationResult<bool> UpdateCurrentData(int currentYear, int currentSemester);
    }
}
