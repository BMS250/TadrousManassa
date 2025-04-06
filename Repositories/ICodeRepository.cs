using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public interface ICodeRepository
    {
        public OperationResult<string> GetCode(string lectureId);

        public HashSet<string> GenerateCodes(int count, string lectureId);
    }
}
