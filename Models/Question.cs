using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
//using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Quiz Quiz { get; set; }

        public string? AnswerId { get; set; }

        [ForeignKey("AnswerId")]
        public Choice Answer { get; set; }

        public ICollection<Choice> Choices { get; set; } = [];
    }
}