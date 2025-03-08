namespace TadrousManassa.Repositories
{
    public interface ICodeRepository
    {
        public void GenerateCodes(int count, string lectureId);
    }
}
