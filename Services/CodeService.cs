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
        public void GenerateCodes(int count, string lectureId)
        {
            _codeRepository.GenerateCodes(count, lectureId);
        }
    }
}
