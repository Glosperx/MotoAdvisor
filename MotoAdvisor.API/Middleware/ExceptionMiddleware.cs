using System.Net;
using System.Text.Json;

namespace MotoAdvisor.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle status codes set by the pipeline (auth, routing, etc.)
            if (!context.Response.HasStarted)
            {
                await HandleStatusCodeAsync(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteJsonResponseAsync(context, HttpStatusCode.InternalServerError, "An internal error occurred");
        }
    }

    private static async Task HandleStatusCodeAsync(HttpContext context)
    {
        var message = context.Response.StatusCode switch
        {
            404 => "Resource not found",
            401 => "Unauthorized",
            403 => "Forbidden",
            500 => "An internal error occurred",
            _   => null
        };

        if (message is not null)
            await WriteJsonResponseAsync(context, (HttpStatusCode)context.Response.StatusCode, message);
    }

    private static async Task WriteJsonResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            statusCode = (int)statusCode,
            message,
            timestamp = DateTime.UtcNow
        });

        await context.Response.WriteAsync(body);
    }
}
