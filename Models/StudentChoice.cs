using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class StudentChoice
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [Required]
        public string QuestionId { get; set; }
        
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public string ChoiceId { get; set; }

        [ForeignKey("ChoiceId")]
        public Choice Choice { get; set; }
    }
}
