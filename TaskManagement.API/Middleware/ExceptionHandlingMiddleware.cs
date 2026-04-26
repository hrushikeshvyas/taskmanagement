using TaskManagement.Shared.Exceptions;
using TaskManagement.Shared.Results;
using Microsoft.Extensions.Hosting;

namespace TaskManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        string GetFullMessages(Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            var current = ex;
            while (current != null)
            {
                sb.AppendLine(current.Message);
                current = current.InnerException;
            }
            return sb.ToString().Trim();
        }

        var be = exception as BusinessException;
        var isBusiness = be != null;
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = isBusiness ? be.Message : "An internal server error occurred.",
            Errors = isBusiness ? be.Errors : new List<string> { exception.Message }
        };

        if (_env.IsDevelopment() && !isBusiness)
        {
            response.Errors = new List<string>
            {
                GetFullMessages(exception),
                exception.StackTrace ?? string.Empty
            };
        }

        context.Response.StatusCode = isBusiness ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsJsonAsync(response);
    }
}