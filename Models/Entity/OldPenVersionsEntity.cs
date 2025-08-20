using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class OldPenVersionsEntity : BaseEntity
    {
        public string ? HTML { get; set; }
        public string? CSS { get; set; }
        public string? JS { get; set; }
        public int Version { get; set; }

        public int? PenId { get; set; }
        
        [JsonIgnore]
        public PenEntity? Pen { get; set; }
    }
}
