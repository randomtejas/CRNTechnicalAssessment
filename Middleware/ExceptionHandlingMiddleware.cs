using System.Net;
using System.Text.Json;

namespace SDTechnicalAssessment.Middleware
{
    // This middleware catches unhandled exceptions
    // from anywhere in the API pipeline.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue processing the request.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the actual exception for developers.
                _logger.LogError(
                    ex,
                    "An unhandled exception occurred while processing the request.");

                // Send a standard 500 response to the client.
                await HandleExceptionAsync(context);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context)
        {
            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType =
                "application/json";

            var response = new
            {
                statusCode = 500,
                message = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}