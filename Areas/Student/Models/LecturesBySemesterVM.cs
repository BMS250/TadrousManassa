namespace TadrousManassa.Areas.Student.Models
{
    public class LecturesBySemesterVM
    {
        // The key is the unit name, and the value is a list of lectures for that semester.
        public Dictionary<int, List<LectureVM>> LecturesOfSemestersByUnits { get; set; }
        public List<TopStudentsScores> TopStudentsScores { get; set; }
    }
}
