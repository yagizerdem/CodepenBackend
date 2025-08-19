using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public required string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public required string LastName { get; set; }

        [EmailAddress]
        [Required]
        public required string Email { get; set; }  
    
        public required string Password { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public required string UserName { get; set; }
    
    }
}
