namespace TadrousManassa.Areas.Student.Models
{
    public class LectureListViewModel
    {
        // The key is the unit name, and the value is a list of lectures for that unit.
        public Dictionary<string, List<LectureViewModel>> LecturesOfUnits { get; set; }
    }
}
