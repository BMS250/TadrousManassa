using TadrousManassa.Models;

namespace TadrousManassa.Services.IServices
{
    public interface ICodeService
    {
        public OperationResult<string> GetCode(string lectureId);

        public HashSet<string> GenerateCodes(int count, string lectureId);
    }
}
