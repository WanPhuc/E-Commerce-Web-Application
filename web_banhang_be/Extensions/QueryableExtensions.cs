using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Common;

namespace WebBanHang.Extensions
{
    public static class QueryableExtensions
    {
        //Dem tong record roi lay data dung trang
        public static async Task<PagedResult<T>> ToPagedAsync<T>(this IQueryable<T> query,PagedRequest request)
        {
            var total = await query.CountAsync();
            var data = await query.Skip(request.Skip).Take(request.PageSize).ToListAsync();
            return PagedResult<T>.Create(data, total, request);
        }
    }
}
