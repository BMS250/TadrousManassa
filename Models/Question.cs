using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Question
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Text { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Required]
        public string QuizId { get; set; }

        public float Score { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [Required]
        public string AnswerId { get; set; }

        [ForeignKey("AnswerId")]
        public Choice Answer { get; set; }

        public ICollection<Choice> Choices { get; set; } = [];
    }
}