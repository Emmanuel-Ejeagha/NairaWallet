using FluentValidation;
using NairaWallet.Application.Common.Exceptions;
using NairaWallet.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using ValidationException = FluentValidation.ValidationException;

namespace NairaWallet.WebApi.Middleware;

/// <summary>
/// Global exception handling middleware that returns standardized error responses.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;
        response.ContentType = "application/json";

            (HttpStatusCode statusCode, object message) = exception switch
        {
            ValidationException validationEx => (HttpStatusCode.BadRequest, (object)new { errors = validationEx.Errors }),
            NotFoundException => (HttpStatusCode.NotFound, (object)new { message = exception.Message }),
            ForbiddenAccessException => (HttpStatusCode.Forbidden, (object)new { message = "Access denied" }),
            InsufficientFundsException => (HttpStatusCode.BadRequest, (object)new { message = exception.Message }),
            InvalidOperationException => (HttpStatusCode.BadRequest, (object)new { message = exception.Message }),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, (object)new { message = "Unauthorized" }),
            _ => (HttpStatusCode.InternalServerError, (object)new { message = "An error occurred while processing your request." })
        };

        response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new { error = message });
        await response.WriteAsync(result);
    }
}