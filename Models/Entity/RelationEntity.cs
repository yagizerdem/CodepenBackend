using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class RelationEntity : BaseEntity
    {
    
        public string? FollowerId { get; set; }
        [JsonIgnore]
        public ApplicationUserEntity? Follower { get; set; }
    
        public string? FollowingId { get; set; }

        [JsonIgnore]
        public ApplicationUserEntity? Following { get; set; }

    }
}
