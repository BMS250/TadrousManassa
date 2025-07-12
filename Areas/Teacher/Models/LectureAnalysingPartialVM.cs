using TadrousManassa.Models;

namespace TadrousManassa.Areas.Teacher.Models
{
    public class LectureAnalysingPartialVM
    {
        public List<LectureViewsCountDTO> Lectures { get; set; }
        public Dictionary<string, int> NoWatchers { get; set; }
        public Dictionary<string, Dictionary<string, int>> ViewsCountForStudents { get; set; }
    }
}
