using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Log the request
                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath} started at {StartTime}", 
                    context.Request.Method, 
                    context.Request.Path, 
                    DateTime.UtcNow);

                await next(context);

                // Log the response
                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath} completed with status {StatusCode} at {EndTime}", 
                    context.Request.Method, 
                    context.Request.Path, 
                    context.Response.StatusCode, 
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

            var statusCode = HttpStatusCode.InternalServerError;
            var response = new
            {
                status = (int)statusCode,
                title = "An unexpected error occurred",
                detail = "Something went wrong. Please try again later."
            };

            // Customize response based on exception type
            switch (exception)
            {
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    response = new
                    {
                        status = (int)statusCode,
                        title = "Unauthorized",
                        detail = "You are not authorized to perform this action."
                    };
                    break;
                case DbUpdateException:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        status = (int)statusCode,
                        title = "Database Error",
                        detail = "A database error occurred while processing your request."
                    };
                    break;
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    response = new
                    {
                        status = (int)statusCode,
                        title = "Resource Not Found",
                        detail = "The requested resource was not found."
                    };
                    break;
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        status = (int)statusCode,
                        title = "Invalid Argument",
                        detail = exception.Message
                    };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
} 