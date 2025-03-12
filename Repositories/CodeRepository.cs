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

        public HashSet<string> GenerateCodes(int count, string lectureId)
        {
            // Fetch existing codes from the database to check against
            var existingCodes = _context.StudentLectures
                                        .Select(sl => sl.Code)
                                        .ToHashSet();

            List<StudentLecture> studentLectures = new List<StudentLecture>();
            HashSet<string> generatedCodes = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                string code;
                do
                {
                    code = GenerateAlphanumericCode(8);
                } while (existingCodes.Contains(code) || generatedCodes.Contains(code));

                // Track the newly generated code to avoid duplicates in this batch
                generatedCodes.Add(code);
                existingCodes.Add(code); // Optional: prevents duplicates if the loop runs again

                studentLectures.Add(new StudentLecture
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = code,
                    StudentId = null,
                    LectureId = lectureId
                });
            }

            try
            {
                _context.StudentLectures.AddRange(studentLectures);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Handle unique constraint violation (e.g., log the error)
                // Consider retrying or partial saves if applicable
                return null;
            }

            return generatedCodes;
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
