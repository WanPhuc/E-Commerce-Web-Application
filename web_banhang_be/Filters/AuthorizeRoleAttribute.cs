using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Filters
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        //dung de kai bao role tren controller hoac action
        //base(typeof(AuthorizeRoleFilter)) : Vì AuthorizeRoleAttribute kế thừa từ TypeFilterAttribute, nó cần chỉ định lớp Filter nào sẽ chịu trách nhiệm xử lý logic.
        public AuthorizeRoleAttribute(params string[] roles) : base(typeof(AuthorizeRoleFilter))
        {
            //truyen du lieu tu attribute vao filter
            Arguments = new object[] { roles };
        }
    }
}
