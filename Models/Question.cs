using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Question
    {
        [Key]
        public string Id { get; set; }

        public string Text { get; set; }

        public string? Image { get; set; }

        public string QuizId { get; set; }

        public float Score { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        public string AnswerId { get; set; }

        [ForeignKey("AnswerId")]
        public Choice Answer { get; set; }

        public ICollection<Choice> Choices { get; set; } = [];
    }
}