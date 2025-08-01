﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TadrousManassa.Models
{
    public class Choice
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(255)]
        public string? Text { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Required]
        public string QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        [JsonIgnore]
        public Question Question { get; set; }
    }
}