using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class FollowRequest : BaseEntity
    {
        public string? SenderId { get; set; }
        public ApplicationUserEntity? Sender { get; set; } = null!;

        public string? ReceiverId { get; set; }
        public ApplicationUserEntity? Receiver { get; set; } = null!;
    }
}
