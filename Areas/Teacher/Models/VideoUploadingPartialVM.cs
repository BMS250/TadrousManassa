using TadrousManassa.Models;

namespace TadrousManassa.Areas.Teacher.Models
{
    public class VideoUploadingPartialVM
    {
        public IFormFile? Video { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public string? Unit { get; set; }
        public int? Price { get; set; }
        public string? SheetPath { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
