using Microsoft.AspNetCore.Mvc;

namespace VanFucVN.Core.Web.Filters;

public class AuthorizeRoleAttribute : TypeFilterAttribute
{
    public AuthorizeRoleAttribute(params string[] roles) : base(typeof(AuthorizeRoleFilter))
    {
        Arguments = new object[] { roles };
    }
}
