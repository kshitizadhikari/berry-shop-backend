using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace Api.Middlewares;

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            NotFoundException e => (HttpStatusCode.NotFound, e.Message),
            BadRequestException e => (HttpStatusCode.BadRequest, e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized, e.Message),
            ConflictException e => (HttpStatusCode.Conflict, e.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            statusCode = (int)statusCode,
            message,
            traceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(body);
    }
}