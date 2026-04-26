using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;

namespace TaskManagement.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] ExcludedPaths = 
    {
        "/swagger",
        "/swagger/",
        "/swagger/ui",
        "/swagger/index.html",
        "/swagger/v1",
        "/swagger/v1/swagger.json",
        "/favicon.ico"
    };

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip tenant validation for excluded paths
        if (IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.ContainsKey(TenantConstants.TenantIdHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = $"Missing required header: {TenantConstants.TenantIdHeader}"
            });
            return;
        }

        var tenantId = context.Request.Headers[TenantConstants.TenantIdHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(tenantId) ||
            tenantId.Length < TenantConstants.MinTenantIdLength ||
            tenantId.Length > TenantConstants.MaxTenantIdLength)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Message = $"Invalid {TenantConstants.TenantIdHeader} header value."
            });
            return;
        }

        context.Items[TenantConstants.TenantIdHeader] = tenantId;
        await _next(context);
    }

    private static bool IsExcludedPath(PathString path)
    {
        var pathValue = path.Value ?? string.Empty;
        return ExcludedPaths.Any(excluded => pathValue.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }
}