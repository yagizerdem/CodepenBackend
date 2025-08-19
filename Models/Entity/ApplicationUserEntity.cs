using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class ApplicationUserEntity : IdentityUser
    {

        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";


        // base entity properties 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public EntityStatus Status { get; set; } = EntityStatus.Active;
    
        public ICollection<PenEntity> Pens { get; set; } = new List<PenEntity>();

        public ICollection<PenLikeEntity> Likes { get; set; } = new List<PenLikeEntity>();
    }
}
