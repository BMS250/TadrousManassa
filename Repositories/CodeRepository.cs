using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TadrousManassa.Data;
using TadrousManassa.Models;

namespace TadrousManassa.Repositories
{
    public class CodeRepository : ICodeRepository
    {
        private readonly ApplicationDbContext _context;

        public CodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void GenerateCodes(int count, string lectureId)
        {
            List<StudentLecture> studentLectures = new List<StudentLecture>();

            for (int i = 0; i < count; i++)
            {
                studentLectures.Add(new StudentLecture
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = GenerateAlphanumericCode(8),
                    StudentId = null,
                    LectureId = lectureId
                });
            }

            _context.StudentLectures.AddRange(studentLectures);
            _context.SaveChanges();
        }

        static string GenerateAlphanumericCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            byte[] buffer = new byte[length];
            RandomNumberGenerator.Fill(buffer);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[buffer[i] % chars.Length]);
            }
            return result.ToString();
        }
    }
}
