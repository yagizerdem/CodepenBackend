using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models.Entity;

namespace CodePen.Seeder
{
    public static class PenCommentSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUserEntity> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUserEntity>>();
            ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            List<ApplicationUserEntity> users = userManager.Users.ToList();
            List<PenEntity> pens = dbContext.Pens.ToList();

            if (dbContext.PenComments.Count() > 30) return;
            Random random = new();

            foreach (var pen in pens)
            {
                int commentCount = random.Next(5, 10);
                List<PenCommentEntity> comments = new();

                for (int i = 0; i < commentCount; i++)
                {
                    var newComment = new PenCommentEntity
                    {
                        Content = $"This is comment {i + 1} on pen {pen.Title}",
                        UserId = users[random.Next(users.Count)].Id,
                        PenId = pen.Id,
                        Status = Models.Enums.EntityStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };  
                    comments.Add(newComment);
                    pen.Comments.Add(newComment);
                }

               dbContext.PenComments.AddRange(comments);
                

            }

            dbContext.SaveChanges();

        }
    }
}
