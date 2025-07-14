using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Choice
    {
        [Key]
        public string Id { get; set; }

        public string Text { get; set; }

        public string QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}