using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.DTO;
using Models.Entity;
using Service;
using Service.Business;
using Bogus;

namespace TestService
{
    public class ServiceBase
    {
        protected readonly ServiceProvider _serviceProvider;

        public ServiceBase()
        {
            _serviceProvider = BuildServiceProvider();
        }

        protected ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));


            services.AddScoped<ApplicationUserService>();
            services.AddScoped<ApplicationUserRelatedLogic>();
            services.AddScoped<PenService>();
            services.AddScoped<PenRelatedLogic>();
            services.AddScoped<PenCommentRelatedLogic>();
            services.AddScoped<PenCommentService>();
            services.AddScoped<RelationRelatedLogic>();
            services.AddScoped<RelationService>();

            // add auto mapper
            services.AddAutoMapper(c => { }, typeof(MappingProfile).Assembly);



            services.AddAuthentication();

            services.AddIdentityCore<ApplicationUserEntity>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager() 
            .AddDefaultTokenProviders();

            return services.BuildServiceProvider();

        }

        public void SeedAppUsersFast()
        {
            var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var hasher = new PasswordHasher<ApplicationUserEntity>();

            var faker = new Faker<ApplicationUserEntity>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => $"user{f.IndexFaker}@example.com")
                .RuleFor(u => u.UserName, (f, u) => $"user{f.IndexFaker}")
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper());

            var users = faker.Generate(1000);

            //foreach (var user in users)
            //{
            //    user.PasswordHash = hasher.HashPassword(user, "12345aA!");
            //    user.SecurityStamp = Guid.NewGuid().ToString();
            //}

            db.ApplicationUsers.AddRange(users);
            db.SaveChanges();
        }




    }


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, ApplicationUserEntity>();

            CreateMap<CreatePenDTO, PenEntity>()
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.OldVersions, opt => opt.Ignore());

            CreateMap<UpdatePenDTO, PenEntity>()
               .ForAllMembers(opt =>
                   opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PenEntity, OldPenVersionsEntity>().
                ForMember(dest => dest.Id, opt => opt.Ignore()); // identity insert is off by default in .net


            CreateMap<CreatePenCommentDTO, PenCommentEntity>();
        }
    }
}
