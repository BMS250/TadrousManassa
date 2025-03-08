using TadrousManassa.Models;

namespace TadrousManassa.Areas.Teacher.Models
{
    public class AdminVM
    {
        public int CurrentYear { get; set; }
        public int CurrentSemester { get; set; }
        public Lecture Input { get; set; }
    }
}
