using System.Net;
using System.Text.Json;
using Accounts.Application;
using Accounts.Domain;

namespace Accounts.Api;

/// <summary>Convierte excepciones de negocio en respuestas ProblemDetails uniformes.</summary>
public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            var (status, title) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
                ConflictException => (HttpStatusCode.Conflict, "Conflicto"),
                ValidationException or DomainException => (HttpStatusCode.UnprocessableEntity, "Regla de negocio incumplida"),
                _ => (HttpStatusCode.InternalServerError, "Error interno")
            };
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { type = "about:blank", title, status = (int)status, detail = exception.Message, traceId = context.TraceIdentifier }));
        }
    }
}