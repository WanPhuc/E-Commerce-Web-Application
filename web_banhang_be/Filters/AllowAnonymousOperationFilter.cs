using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace WebBanHang.Filters
{
    public class AllowAnonymousOperationFilter :  IOperationFilter
    {
        public void Apply(OpenApiOperation operation,OperationFilterContext filterContext)
        {
            var hasAllowAnonymous = filterContext.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (hasAllowAnonymous)
            {
                operation.Description ??= "";
                operation.Description += "<b>🔓 This endpoint allows anonymous access (no JWT required).</b>";
            }
        }
    }
}
