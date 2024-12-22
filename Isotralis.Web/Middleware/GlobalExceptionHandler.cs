using Microsoft.AspNetCore.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace Isotralis.Web.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException oce)
        {
            _logger.LogWarning(oce, "An operation was canceled");
            return true;
        }

        if (exception is InvalidOperationException && exception.InnerException is OracleException oracleEx && oracleEx.Number == 50000)
        {
            // Log and redirect for ORA-50000
            _logger.LogError(oracleEx, "An application-specific error occurred (ORA-50000).");
            httpContext.Response.Redirect("/Error/Index/50000");
            await httpContext.Response.CompleteAsync(); // Ensure no further processing
            return true;
        }

        // Log the unexpected error
        _logger.LogError(exception, "An unexpected error occurred.");

        if (_env.IsDevelopment())
        {
            // Developer exception page in development
            return false; // Let ASP.NET handle it with the developer exception page
        }
        else
        {
            // Generic error handling for production
            httpContext.Response.Redirect("/Error/Index/500");
            await httpContext.Response.CompleteAsync();
            return true;
        }
    }
}