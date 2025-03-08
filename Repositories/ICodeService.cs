namespace TadrousManassa.Repositories
{
    public interface ICodeService
    {
        public void GenerateCodes(int count, string lectureId);
    }
}
