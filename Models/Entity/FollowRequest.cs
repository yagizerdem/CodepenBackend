using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class FollowRequest : BaseEntity
    {
        public string? SenderId { get; set; }
        
        [JsonIgnore]
        public ApplicationUserEntity? Sender { get; set; } = null!;

        public string? ReceiverId { get; set; }
        
        [JsonIgnore]
        public ApplicationUserEntity? Receiver { get; set; } = null!;
        public FollowRequestStatus FollowRequestStatus { get; set; } = FollowRequestStatus.Pending; 
    }
}
