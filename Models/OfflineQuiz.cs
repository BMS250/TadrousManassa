using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class OfflineQuiz
    {
        public string Id { get; set; }
        public int WeekNumber { get; set; }
        public float Score { get; set; }
        public float TotalScore { get; set; } = 10;
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
