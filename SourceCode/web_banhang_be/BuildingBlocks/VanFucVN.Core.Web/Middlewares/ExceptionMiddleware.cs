using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VanFucVN.Core.Common.Constants;
using VanFucVN.Core.Common.DTOs;

namespace VanFucVN.Core.Web.Middlewares;

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
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var errorResponse = ApiResponse.Fail("Internal Server Error", StatusCodes.Status500InternalServerError, ErrorCodes.Common.InternalServerError);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
