using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Models.Entity;

namespace CodePen.Seeder
{
    public static class PenSeeder
    {


        public static void Seed(IServiceProvider serviceProvider)
        {
            ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Pens.Any()) return;

            List<ApplicationUserEntity> users = dbContext.ApplicationUsers.ToList();
            List<PenEntity> pens = new();
            Random random = new();

            for (int i = 0; i < 1000; i++)
            {
                var randomUser = users[random.Next(0, users.Count)];
                PenEntity pen = new()
                {
                    Title = $"Pen {i + 1}",
                    Description = $"This is the description for pen {i + 1}.",
                    Author = randomUser,
                    
                };
                pens.Add(pen);
            }

            dbContext.Pens.AddRange(pens);  
            dbContext.SaveChanges();
        }

    }
}
