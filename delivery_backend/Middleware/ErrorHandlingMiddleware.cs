using System.Net;
using System.Text.Json;
using DeliveryBackend.DTOs;

namespace DeliveryBackend.Middleware
{
    /// <summary>
    /// Global error handling middleware.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Middleware entry to handle unhandled exceptions and write a consistent error response.
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation");
                await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        private static async Task WriteError(HttpContext ctx, HttpStatusCode code, string message)
        {
            ctx.Response.StatusCode = (int)code;
            ctx.Response.ContentType = "application/json";

            var payload = new
            {
                theme = new { primary = "#2563EB", secondary = "#F59E0B", error = "#EF4444", text = "#111827" },
                error = new ErrorResponse(message)
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await ctx.Response.WriteAsync(json);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Registers the global error handling middleware.
        /// </summary>
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
