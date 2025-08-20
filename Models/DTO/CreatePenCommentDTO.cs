using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CreatePenCommentDTO
    {
        [Required]
        public required int PenId { get; set; }
        
        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public required string Content { get; set; }
    
    }
}
