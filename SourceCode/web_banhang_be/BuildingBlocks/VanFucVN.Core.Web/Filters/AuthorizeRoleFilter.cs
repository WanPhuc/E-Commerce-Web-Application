using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using VanFucVN.Core.Common.Constants;
using VanFucVN.Core.Common.DTOs;
using VanFucVN.Core.Common.Services;

namespace VanFucVN.Core.Web.Filters;

public class AuthorizeRoleFilter : IAsyncActionFilter
{
    private readonly string[] _allowRole;
    private readonly ICommonService _commonService;

    public AuthorizeRoleFilter(string[] allowRole, ICommonService commonService)
    {
        _allowRole = allowRole;
        _commonService = commonService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var roleClaim = context.HttpContext.User.FindFirst("role")?.Value
            ?? context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleClaim))
        {
            context.Result = new ObjectResult(ApiResponse.Fail("Unauthorized.", 401, ErrorCodes.Common.Unauthorized));
            return;
        }

        var requiredWeight = _allowRole.Select(r => AppRoles.RoleWeights.GetValueOrDefault(r, 0)).DefaultIfEmpty(0).Max();
        var userWeight = AppRoles.RoleWeights.GetValueOrDefault(roleClaim, 0);

        if (userWeight >= requiredWeight)
        {
            await next();
            return;
        }

        context.Result = new ObjectResult(ApiResponse.Fail("You do not have permission to perform this action.", 403, ErrorCodes.Common.Forbidden));
    }
}
