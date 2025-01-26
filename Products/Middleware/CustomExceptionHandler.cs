using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Products.Middleware
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "$Exception occurred: {message}", exception.Message);
            var problemDetails = exception switch
            {
                ArgumentException ex => CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Invalid argument",
                    ex.Message),

                _ => CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred")
            };

            httpContext.Response.StatusCode = problemDetails.Status ?? 500;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        private static ProblemDetails CreateProblemDetails(int status, string title, string detail) => new()
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}"
        };
    }
}