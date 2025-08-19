using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class UpdatePenDTO
    {
        public string? HTML { get; set; }
        public string? CSS { get; set; }
        public string? JS { get; set; }
        [MinLength(3)]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(10000)]
        public string? Description { get; set; }
    }
}
