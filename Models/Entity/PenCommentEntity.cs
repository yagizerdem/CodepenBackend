using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class PenCommentEntity : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? UserId { get; set; }
        
        [JsonIgnore]
        public ApplicationUserEntity? User { get; set; } = null!;
        public int? PenId { get; set; }

        [JsonIgnore]
        public PenEntity? Pen { get; set; } = null!;

    }

}
