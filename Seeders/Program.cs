using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Entity;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Seeders
{
    internal class Program
    {
        private static ServiceCollection services = new();
        private static HostApplicationBuilder builder;
        private static ServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            Init();

            //SeedLikesToUsersPosts();
            //SeedCommentsToUsersPosts();

            //SeedFollowersToUser();
            //SeedFollowingsToUser();

            //SeedFollowRequestsToUser();
        }

        private static void Init()
        {
            builder = Host.CreateApplicationBuilder();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


            // register services
            services.AddScoped<ApplicationDbContext>();

            // build service provider
            serviceProvider = services.BuildServiceProvider();
        }

        private static void SeedLikesToUsersPosts()
        {
            ApplicationDbContext db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            ApplicationUserEntity user = db.Users.FirstOrDefault(x => x.Email == "yagizerdem@gmail.com") ??
                throw 
                new Exception("user not found"); 
        
            List<ApplicationUserEntity> otherUsers = db.Users
                .Where(x => x.Id != user.Id)
                .ToList();

            List<PenEntity> userPens = db.Pens.Where(x => x.AuthorId == user.Id).ToList();

            Random rnd = new();
            foreach (PenEntity pen in userPens)
            {
                IReadOnlyCollection<ApplicationUserEntity> enumerable = 
                    otherUsers.OrderBy(_ => rnd.Next(999)).Take(50).ToList();

                enumerable.ToList().ForEach(x => 
                {
                    PenLikeEntity like = new()
                    {
                        Pen = pen,
                        PenId = pen.Id,
                        UserId = x.Id,
                        User = x,
                    };
                    db.PenLikes.Add(like);
                });

                db.SaveChanges();
            }

        }

        private static void SeedCommentsToUsersPosts()
        {
            ApplicationDbContext db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            ApplicationUserEntity user = db.Users.FirstOrDefault(x => x.Email == "yagizerdem@gmail.com") ??
                throw
                new Exception("user not found");

            List<ApplicationUserEntity> otherUsers = db.Users
                .Where(x => x.Id != user.Id)
                .ToList();

            List<PenEntity> userPens = db.Pens.Where(x => x.AuthorId == user.Id).ToList();

            Random rnd = new();

            foreach (PenEntity pen in userPens)
            {
                IReadOnlyCollection<ApplicationUserEntity> otherUsersShuffled =
                    otherUsers.OrderBy(_ => rnd.Next(999)).Take(30).ToList();

                otherUsersShuffled.ToList().ForEach(x =>
                {
                    PenCommentEntity comment = new()
                    {
                        Pen = pen,
                        PenId = pen.Id,
                        UserId = x.Id,
                        Content = string.Join(" ", LoremIpsum.Split(" ")
                        .OrderBy(_ => rnd.Next(999))
                        .Take(rnd.Next(5, LoremIpsum.Split(" ").Length))),
                    };
                    db.PenComments.Add(comment);
                });

                db.SaveChanges();
            }
        }


        private static void SeedFollowersToUser()
        {
            ApplicationDbContext db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            ApplicationUserEntity user = db.Users.FirstOrDefault(x => x.Email == "yagizerdem@gmail.com") ??
                throw
                new Exception("user not found");

            List<ApplicationUserEntity> otherUsers = db.Users
                .Where(x => x.Id != user.Id)
                .ToList();

            Random rnd = new();

            var sampleUser = otherUsers.OrderBy(_ => rnd.Next(999)).Take(20).ToList();

            sampleUser.ForEach(x =>
            {
                RelationEntity relation = new()
                {
                    Follower = x,
                    Following = user,
                    FollowerId = x.Id,
                    FollowingId = user.Id,
                };
                db.Relations.Add(relation);
            });

            db.SaveChanges();

        }

        private static void SeedFollowingsToUser()
        {
            ApplicationDbContext db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            ApplicationUserEntity user = db.Users.FirstOrDefault(x => x.Email == "yagizerdem@gmail.com") ??
                throw
                new Exception("user not found");

            List<ApplicationUserEntity> otherUsers = db.Users
                .Where(x => x.Id != user.Id)
                .ToList();

            Random rnd = new();

            var sampleUser = otherUsers.OrderBy(_ => rnd.Next(999)).Take(35).ToList();

            sampleUser.ForEach(x =>
            {
                RelationEntity relation = new()
                {
                    Follower = user,
                    Following = x,
                    FollowerId = user.Id,
                    FollowingId = x.Id,
                };
                db.Relations.Add(relation);
            });

            db.SaveChanges();
        }

        private static void SeedFollowRequestsToUser()
        {
            ApplicationDbContext db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            ApplicationUserEntity user = db.Users.FirstOrDefault(x => x.Email == "yagizerdem@gmail.com") ??
                throw
                new Exception("user not found");

            List<ApplicationUserEntity> otherUsers = db.Users
            .Where(x => x.Id != user.Id)
            .ToList();

            Random rnd = new();

            var sampleUser = otherUsers.OrderBy(_ => rnd.Next(999)).Take(35).ToList();
        
            sampleUser.ForEach(x =>
            {
                FollowRequest followRequest = new()
                {
                    Receiver = user,
                    ReceiverId = user.Id,
                    Sender = x,
                    SenderId = x.Id,
                };
                db.FollowRequests.Add(followRequest);
            });

            db.SaveChanges();
        }

        private static string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    }
}
