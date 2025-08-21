using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class RelationEntity : BaseEntity
    {
    
        public string? FollowerId { get; set; }
        public ApplicationUserEntity? Follower { get; set; }
    
        public string? FollowingdId { get; set; }

        public ApplicationUserEntity? Following { get; set; }

    }
}
