using Microsoft.AspNetCore.Http;

namespace VanFucVN.Core.Common.Services;

public class CommonService : ICommonService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string UserIdKey = "UserId";

    public CommonService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetUserId(Guid userId)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx != null)
        {
            ctx.Items[UserIdKey] = userId;
        }
    }

    public Guid GetUserId()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx?.Items.TryGetValue(UserIdKey, out var value) == true && value is Guid id)
        {
            return id;
        }
        return Guid.Empty;
    }
}
