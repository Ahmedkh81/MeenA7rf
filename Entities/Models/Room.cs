using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum RoomSize
    {
        TwoPlayers = 2,
        FourPlayers = 4
    }
    public class Room
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CreatedById { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public RoomSize MaxPlayers { get; set; }
        public ICollection<RoomPlayer> Players { get; set; } = new List<RoomPlayer>();

        public Room()
        {
            Code = Guid.NewGuid().ToString().Substring(0, 6);
        }
    }
}
