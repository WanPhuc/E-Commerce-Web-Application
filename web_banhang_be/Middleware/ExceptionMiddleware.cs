using Serilog;
using WebBanHang.Models.Common;
namespace WebBanHang.Middleware
{
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
                Log.Error(ex, "Unhandled exception occurred");
                context.Response.Clear();//xoa tat ca du lieu da duoc ghi vao response truoc do
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = ApiResponse.Fail("Internal Server Error", StatusCodes.Status500InternalServerError, ErrorCodes.Common.InternalServerError);
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
