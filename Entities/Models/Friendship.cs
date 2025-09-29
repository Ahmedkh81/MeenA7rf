using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Blocked
    }
    public class Friendship
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = null!;
        public ApplicationUser Player { get; set; } = null!;
        public string FriendId { get; set; } = null!;
        public ApplicationUser Friend { get; set; } = null!;
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    }
}
