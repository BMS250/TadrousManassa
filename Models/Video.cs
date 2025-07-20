using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadrousManassa.Models
{
    public class Video
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Path { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        // Optional if a quiz is not always required after a video
        public string? QuizId { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }

        [Required]
        public string LectureId { get; set; }

        [ForeignKey("LectureId")]
        public virtual Lecture Lecture { get; set; }

        public int Order { get; set; } = 0;

        public int ViewsCount { get; set; } = 0;
    }
}
