using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TadrousManassa.Data;
using TadrousManassa.Models;
using TadrousManassa.Repositories;

namespace TadrousManassa.Services
{
    public class CodeService : ICodeService
    {
        private readonly ICodeRepository _codeRepository;

        public CodeService(ICodeRepository codeRepository)
        {
            _codeRepository = codeRepository;
        }

        public OperationResult<string> GetCode(string lectureId)
        {
            return _codeRepository.GetCode(lectureId);
        }
        public HashSet<string> GenerateCodes(int count, string lectureId)
        {
            return _codeRepository.GenerateCodes(count, lectureId);
        }
    }
}
