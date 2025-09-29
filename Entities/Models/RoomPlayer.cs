using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RoomPlayer
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public string PlayerId { get; set; } = null!;
        public ApplicationUser Player { get; set; } = null!;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
