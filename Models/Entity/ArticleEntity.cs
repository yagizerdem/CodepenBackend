using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class ArticleEntity : BaseEntity
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(100000)]
        public string FullText { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUserEntity  Author { get; set; }
        public DateTime PlannedPublishDate { get; set; } // must be future date
        public ArticleVisibility Visibility { get; set; } = ArticleVisibility.Public;
        
        [JsonIgnore]
        public byte[]? CoverImage { get; set; } // optional

        [JsonIgnore]
        public ICollection<BookMark> BookMarks { get; set; }
    }
}
