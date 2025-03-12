using TadrousManassa.Models;

namespace TadrousManassa.Services
{
    public interface ICodeService
    {
        public HashSet<string> GenerateCodes(int count, string lectureId);
    }
}
