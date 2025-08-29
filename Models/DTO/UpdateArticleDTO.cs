using Microsoft.AspNetCore.Http;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class UpdateArticleDTO
    {
        [Required]
        public int Id { get; set; } // article id to be updated

        [MinLength(2)]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(100000)]
        public string? FullText { get; set; }

        [MaxLength(500)]
        public string? Abstract { get; set; } // short summary of the article    

        public ArticleVisibility? Visibility { get; set; } 
        public IFormFile? CoverImage { get; set; } 
    }
}
