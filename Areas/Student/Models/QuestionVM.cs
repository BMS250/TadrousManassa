using TadrousManassa.Models;

namespace TadrousManassa.Areas.Student.Models
{
    public class QuestionVM
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string? Image { get; set; }
        public List<ChoiceVM> Choices { get; set; } = new();
    }
}
