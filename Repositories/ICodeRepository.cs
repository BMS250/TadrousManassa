using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface ICodeRepository
    {
        public HashSet<string> GenerateCodes(int count, string lectureId);
    }
}
