using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories.IRepositories;

namespace TadrousManassa.Repositories
{
    public class AppSettingsRepository : IAppSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public AppSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationSettings GetCurrentData()
        {
            return _context.ApplicationSettings.FirstOrDefault();
        }

        public OperationResult<bool> UpdateCurrentData(int currentYear, int currentSemester)
        {
            try
            {
                var appSettings = _context.ApplicationSettings.FirstOrDefault();
                appSettings.CurrentYear = currentYear;
                appSettings.CurrentSemester = currentSemester;
                _context.SaveChanges();
                return OperationResult<bool>.Ok(true, "Data updated successfully");
            }
            catch
            {
                return OperationResult<bool>.Fail("Failed to update data");
            }
        }
    }
}
