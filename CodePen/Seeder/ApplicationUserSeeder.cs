using DataAccess;
using Microsoft.AspNetCore.Identity;
using Models.Entity;

namespace CodePen.Seeder
{
    public static class ApplicationUserSeeder
    {

        public static void Seed(IServiceProvider serviceProvider)
        {
            ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            UserManager<ApplicationUserEntity> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUserEntity>>();

            if (dbContext.ApplicationUsers.Count() > 30) return;
            List<ApplicationUserEntity> users = new();
            for (int i = 0; i < 200; i++)
            {
                ApplicationUserEntity user = new()
                {
                    UserName = $"user{i + 1}",
                    Email = $"user{i + i}.gmail.com",
                    FirstName = $"FirstName{i + 1}",
                    LastName = $"LastName{i + 1}",
                };

                users.Add(user);
            }

            string basePassword = "12345aA!";
            foreach (var user in users)
            {

                userManager.CreateAsync(user, basePassword).GetAwaiter().GetResult();
            }

        }
    

    }
}
