using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum MatchOutcome
    {
        Winner,
        Loser
    }
    public class MatchResult
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public string PlayerId { get; set; } = null!;
        public ApplicationUser Player { get; set; } = null!;
        public int FinalScore { get; set; }
        public MatchOutcome MatchOutcome { get; set; }
        public DateTime FinishedAt { get; set; } = DateTime.UtcNow;
    }
}
