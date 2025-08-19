using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<ApplicationUserEntity> ApplicationUsers { get; set; }  

        public DbSet<PenEntity> Pens { get; set; }

        public DbSet<OldPenVersionsEntity> OldPenVersions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OldPenVersionsEntity>()
                .HasOne(v => v.Pen)
                .WithMany(p => p.OldVersions)
                .OnDelete(DeleteBehavior.SetNull);  
        }


    }
}
