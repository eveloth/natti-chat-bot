using System.ComponentModel.DataAnnotations;
using NattiChatBot.Exceptions;
using Npgsql;

namespace NattiChatBot.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (ApiException e)
        {
            _logger.LogError(e.Message);

            context.Response.StatusCode = e.Message.Contains("not found") ? 404 : 400;
            await context.Response.WriteAsJsonAsync(e.Message);
        }
        catch (NpgsqlException e)
        {
            _logger.LogCritical("Error talking to the database: {Error}", e);

            context.Response.StatusCode = 500;
        }
        catch (ValidationException e)
        {
            var err = e.ValidationResult.ErrorMessage;
            _logger.LogError(err);

            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(err);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Unknown error occured: {Error}", e);

            context.Response.StatusCode = 500;
        }
    }
}