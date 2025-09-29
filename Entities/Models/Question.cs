using Microsoft.CodeAnalysis.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum Level
    {
        Easy,
        Medium,
        Hard
    }
    public class Question
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        public Level Difficulty { get; set; }
        public int Points { get; set; }
        public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    }
}
