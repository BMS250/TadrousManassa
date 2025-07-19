namespace TadrousManassa.Areas.Student.Models
{
    public class VideoDetailsDTO
    {
        public string Id { get; set; }
        public string LectureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string SheetPath { get; set; }
        public int Order { get; set; } = 0;
    }
}
