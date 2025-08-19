using System.Net;
using System.Text.Json;
using Models.Exceptions;
using Models.ResponseTypes;

namespace CodePen.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // continue pipeline
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Service exception: {Message}", ex.Message);
                if (ex.IsOperational)
                    await WriteErrorResponse(context, ex.Message, ex.Errors.ToList(), ex.StatusCode);
                else
                    await WriteErrorResponse(context, "Internal Server Error", [], HttpStatusCode.InternalServerError);

            }
            catch (ServiceException ex)
            {
                // Known service-level error (business rule)
                _logger.LogWarning(ex, "Service exception: {Message}", ex.Message);
                if (ex.IsOperational)
                    await WriteErrorResponse(context, ex.Message, ex.Errors, HttpStatusCode.BadRequest);
                else
                    await WriteErrorResponse(context, "Internal Server Error", [], HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                // Unexpected exception
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorResponse(context, "An unexpected error occurred.", [], HttpStatusCode.InternalServerError);
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, string message, List<string> errors, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.ErrorResponse(
                message: message,
                errors: errors,
                statusCode: statusCode);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
