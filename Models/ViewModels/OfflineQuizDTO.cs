using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models.ViewModels
{
    public class OfflineQuizDTO
    {
        public int WeekNumber { get; set; }
        public float Score { get; set; }
        public float TotalScore { get; set; } = 10;
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string StudentId { get; set; }
    }
}
