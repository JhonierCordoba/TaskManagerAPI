using System.Net;
using System.Text.Json;
using FluentValidation;
using TaskManager.Application.Common.Exceptions;

namespace TaskManager.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedException => (HttpStatusCode.Forbidden, exception.Message),
            ValidationException ex => (HttpStatusCode.BadRequest, string.Join(", ", ex.Errors.Select(e => e.ErrorMessage))),
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(result);
    }
}