using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity>
    {

        public DbSet<ApplicationUserEntity> ApplicationUsers { get; set; }  
        public DbSet<PenEntity> Pens { get; set; }
        public DbSet<OldPenVersionsEntity> OldPenVersions { get; set; }
        public DbSet<PenLikeEntity> PenLikes { get; set; }
        public DbSet<PenCommentEntity> PenComments { get; set; }
        public DbSet<MediaWrapper> MediaWrapper { get; set; } 
        public DbSet<FollowRequest> FollowRequests { get; set; }

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

            modelBuilder.Entity<PenLikeEntity>()
                .HasOne(v => v.Pen)
                .WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<PenLikeEntity>(entity =>
            {
                entity.HasOne(pl => pl.Pen)
                      .WithMany(p => p.Likes)
                      .HasForeignKey(pl => pl.PenId)
                      .OnDelete(DeleteBehavior.SetNull);   

                entity.HasOne(pl => pl.User)
                      .WithMany(u => u.Likes)
                      .HasForeignKey(pl => pl.UserId)
                      .OnDelete(DeleteBehavior.SetNull);   
            });

            modelBuilder.Entity<PenCommentEntity>(entity =>
            {
                entity.HasOne(pc => pc.Pen)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(pc => pc.PenId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(pc => pc.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(pc => pc.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<FollowRequest>(entity =>
            {
                entity.HasOne(f => f.Sender)
                    .WithMany(u => u.SentFollowRequests)
                    .HasForeignKey(f => f.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.Receiver)
                    .WithMany(u => u.ReceivedFollowRequests)
                    .HasForeignKey(f => f.ReceiverId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

        }


    }
}
