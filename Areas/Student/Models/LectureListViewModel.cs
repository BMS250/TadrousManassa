namespace TadrousManassa.Areas.Student.Models
{
    public class LectureListViewModel
    {
        // The key is the unit name, and the value is a list of lectures for that semester.
        public Dictionary<int, List<LectureViewModel>> LecturesOfSemesters { get; set; }
    }
}
