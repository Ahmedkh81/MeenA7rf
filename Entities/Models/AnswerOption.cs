using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
