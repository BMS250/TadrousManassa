namespace TadrousManassa.Areas.Student.Models
{
    public class QuizDetailsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TimeHours { get; set; } = 1;
        public int TimeMinutes { get; set; } = 0;
        public int NumOfRemainingAttempts { get; set; }
    }
}
