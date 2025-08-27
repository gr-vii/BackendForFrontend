using System.Net;
using System.Text.Json;
using FluentValidation;

namespace BackendForFrontend.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred processing request {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                           ?? context.TraceIdentifier;

        var problemDetails = exception switch
        {
            ValidationException validationEx => new
            {
                Type = "https://api.BackendForFrontend.com/problems/validation-error",
                Title = "Validation Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage)),
                Instance = context.Request.Path.Value,
                TraceId = correlationId
            },
            _ => new
            {
                Type = "https://api.BackendForFrontend.com/problems/internal-error",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An error occurred while processing your request",
                Instance = context.Request.Path.Value,
                TraceId = correlationId
            }
        };

        context.Response.StatusCode = problemDetails.Status;
        context.Response.ContentType = "application/problem+json";
        context.Response.Headers.Append("X-Correlation-ID", correlationId);

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
