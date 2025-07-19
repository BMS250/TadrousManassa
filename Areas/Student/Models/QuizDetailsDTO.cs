namespace TadrousManassa.Areas.Student.Models
{
    public class QuizDetailsDTO
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public int TimeHours { get; set; } = 1;
        public int TimeMinutes { get; set; } = 0;
    }
}
