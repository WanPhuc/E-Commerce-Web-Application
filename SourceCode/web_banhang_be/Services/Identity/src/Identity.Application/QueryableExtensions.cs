using Microsoft.EntityFrameworkCore;

namespace AuraMart.Identity.Application;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(this IQueryable<T> query, PagedRequest request)
    {
        var total = await query.CountAsync();
        var data = await query.Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return PagedResult<T>.Create(data, total, request);
    }
}
