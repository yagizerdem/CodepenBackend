using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class PenEntity : BaseEntity
    {
    
        public string? HTML { get; set; }
        public string? CSS { get; set; }
        public string? JS { get; set; } 
        public required string Title { get; set; }
        public string? Description { get; set; }

        public string? AuthorId { get; set; }

        public ApplicationUserEntity Author { get; set; } = null!;
        public int Version { get; set; } = 1;

        [JsonIgnore]
        public ICollection<OldPenVersionsEntity> OldVersions { get; set; } = new List<OldPenVersionsEntity>();

        [JsonIgnore]
        public ICollection<PenLikeEntity> Likes { get; set; } = new List<PenLikeEntity>();

        [JsonIgnore]
        public ICollection<PenCommentEntity> Comments { get; set; } = new List<PenCommentEntity>();

    }
}
