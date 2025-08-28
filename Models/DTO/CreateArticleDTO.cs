using Microsoft.AspNetCore.Http;
using Models.Entity;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CreateArticleDTO
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(100000)]
        public string FullText { get; set; }
        public DateTime PlannedPublishDate { get; set; } = DateTime.UtcNow; // must be future date 
        public ArticleVisibility Visibility { get; set; } = ArticleVisibility.Public;
        public IFormFile? CoverImage { get; set; } // optional
    }
}
