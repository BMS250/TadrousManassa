namespace TadrousManassa.Areas.Teacher.Models
{
    public class LectureVM
    {
        public string Description { get; set; }

        public int Grade { get; set; }

        public string Unit { get; set; }

        public int Price { get; set; }

        public string? SheetURL { get; set; }
        
        public string? QuizURL { get; set; }

        public IFormFile Video { get; set; }
    }
}
