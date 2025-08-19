using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class PenLikeEntity : BaseEntity
    {
        public int? PenId { get; set; }

        public PenEntity? Pen { get; set; }

        public string? UserId { get; set; }
        public ApplicationUserEntity? User { get; set; }
    
        
    }
}
