
using CodePen.Middleware;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.ResponseTypes;
using Service;
using System.Net;

namespace CodePen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // database context injection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // configure app user and role
            builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            // customize error messages
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors
                            .Select(e => $"{kvp.Key}: {e.ErrorMessage}"))
                        .ToList();

                    var response = ApiResponse<object?>.ErrorResponse(
                        message: "Validation failed.",
                        statusCode: System.Net.HttpStatusCode.BadRequest,
                        errors: errors
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            // cookie configuration
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = System.Text.Json.JsonSerializer.Serialize(
                            ApiResponse<object?>.ErrorResponse(message: string.Empty, errors: ["Authentication required. Please log in."], statusCode: HttpStatusCode.Unauthorized)
                        );
                        return context.Response.WriteAsync(response);
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = System.Text.Json.JsonSerializer.Serialize(
                            ApiResponse<object?>.ErrorResponse(message: string.Empty, errors: ["Access denied: you dont have the required role."], statusCode: HttpStatusCode.Forbidden)
                        );
                        return context.Response.WriteAsync(response);
                    }
                };
                options.ExpireTimeSpan = TimeSpan.FromDays(90);
            });


            builder.Services.AddScoped<ApplicationUserService>();

            // Register AutoMapper with all profiles in the assembly
            builder.Services.AddAutoMapper(c => { }, typeof(MappingProfile).Assembly);

            var app = builder.Build();

            // handle all unhandled exceptions globally
            app.UseMiddleware<GlobalExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
