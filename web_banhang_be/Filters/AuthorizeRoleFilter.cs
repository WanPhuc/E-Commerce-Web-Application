using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebBanHang.Helpers;
using WebBanHang.Models.Common;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Filters
{
    public class AuthorizeRoleFilter : IAsyncActionFilter
    {
        private readonly string[] _allowRole;
        private readonly ICommonService _commonService;
        private readonly IUserRepository _userRepository;
        public AuthorizeRoleFilter(string[] allowRole, ICommonService commonService, IUserRepository userRepository)
        {
            _allowRole = allowRole;
            _commonService = commonService;
            _userRepository = userRepository;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userID = _commonService.GetUserId();
            var user = await _userRepository.GetByIdAsync(userID, u => u.Role);
            if (user == null)
            {
                context.Result = new ObjectResult(ApiResponse.Fail("Unauthorized.", 401, ErrorCodes.Common.Unauthorized));
                return;
            }
            // lay level cua role cao nhat duoc yeu cau 
            var requiredWeight = _allowRole.Select(r => AppRoles.RoleWeights.GetValueOrDefault(r, 0)).Max();
            //lay level cua user hien tai
            var userWeight = AppRoles.RoleWeights.GetValueOrDefault(user.Role.Name, 0);
            if (userWeight >= requiredWeight)
            {
                await next();
                return;
            }
            context.Result = new ObjectResult(ApiResponse.Fail("You do not have permission to perform this action.", 403, ErrorCodes.Common.Forbidden));
        }
    }
}
