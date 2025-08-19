
using CodePen.Middleware;
using Microsoft.AspNetCore.Mvc;
using Models.ResponseTypes;

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
