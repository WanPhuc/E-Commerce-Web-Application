using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using AuraMart.Identity.Infrastructure.Persistence;

namespace AuraMart.Identity.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : BaseController
{
    private readonly IdentityDbContext _db;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _config;

    public AdminDashboardController(IdentityDbContext db, IHttpClientFactory clientFactory, IConfiguration config)
    {
        _db = db;
        _clientFactory = clientFactory;
        _config = config;
    }

    private void AddInternalHeader(HttpClient client)
    {
        var key = _config["InternalApi:Key"] ?? "dev-internal-key";
        client.DefaultRequestHeaders.Remove("X-Internal-Api-Key");
        client.DefaultRequestHeaders.Add("X-Internal-Api-Key", key);
    }

    // GET /api/v1/admin
    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // 1. Users
        var usersAll = await _db.Users.AsNoTracking().CountAsync();
        var usersLastMonth = await _db.Users.AsNoTracking().CountAsync(u => u.CreatedAt < thisMonthStart);

        // 2. Products from Catalog
        int totalProducts = 0;
        try
        {
            var catalogClient = _clientFactory.CreateClient("catalog");
            AddInternalHeader(catalogClient);
            var prodRes = await catalogClient.GetFromJsonAsync<CountDto>("internal/catalog/products/stats/total");
            totalProducts = prodRes?.Count ?? 0;
        }
        catch { }

        // 3. Sellers from Seller service
        int sellersAll = 0;
        int sellersLastMonth = 0;
        try
        {
            var sellerClient = _clientFactory.CreateClient("seller");
            AddInternalHeader(sellerClient);
            var sAllRes = await sellerClient.GetFromJsonAsync<CountDto>("internal/sellers/count");
            sellersAll = sAllRes?.Count ?? 0;

            var sLastRes = await sellerClient.GetFromJsonAsync<CountDto>($"internal/sellers/stats/created-before?before={Uri.EscapeDataString(thisMonthStart.ToString("o"))}");
            sellersLastMonth = sLastRes?.Count ?? 0;
        }
        catch { }

        // 4. Orders & Revenue from Ordering service
        int ordersAll = 0;
        int ordersLastMonth = 0;
        decimal revenueAll = 0m;
        decimal revenueLastMonth = 0m;
        try
        {
            var orderClient = _clientFactory.CreateClient("ordering");
            AddInternalHeader(orderClient);
            var oRes = await orderClient.GetFromJsonAsync<OrderSummaryDto>("internal/ordering/stats/summary");
            if (oRes != null)
            {
                ordersAll = oRes.TotalOrdersThisMonth;
                ordersLastMonth = oRes.TotalOrdersLastMonth;
                revenueAll = oRes.RevenueThisMonth;
                revenueLastMonth = oRes.RevenueLastMonth;
            }
        }
        catch { }

        var result = new AdminDashboardDto
        {
            TotalProducts = totalProducts,
            TotalSellersThisMonth = sellersAll,
            TotalSellersLastMonth = sellersLastMonth,
            TotalUsersThisMonth = usersAll,
            TotalUsersLastMonth = usersLastMonth,
            TotalOrdersThisMonth = ordersAll,
            TotalOrdersLastMonth = ordersLastMonth,
            RevenueThisMonth = revenueAll,
            RevenueLastMonth = revenueLastMonth
        };

        return Ok(ApiResponse<AdminDashboardDto>.Success(result, "Lấy dữ liệu dashboard thành công"));
    }

    // GET /api/v1/admin/chart?range=Week
    [HttpGet("chart")]
    public async Task<IActionResult> GetChart([FromQuery] string range = "Week")
    {
        var now = DateTime.UtcNow;
        var dataPoints = new List<DashboardChartPointDto>();
        range = string.IsNullOrWhiteSpace(range) ? "Week" : range.Trim();

        DateTime start;
        DateTime end;

        if (range.Equals("Month", StringComparison.OrdinalIgnoreCase))
        {
            range = "Month";
            var year = now.Year;
            start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            end = start.AddYears(1);

            // Base counts
            var baseUsers = await _db.Users.AsNoTracking().CountAsync(u => u.CreatedAt < start);

            // User increments by month
            var userMonths = await _db.Users.AsNoTracking()
                .Where(u => u.CreatedAt >= start && u.CreatedAt < end)
                .GroupBy(u => u.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Month, g => g.Count);

            // Ordering data
            var orderData = await FetchOrdersForChart(start, end);

            int runUsers = 0;
            int runOrders = 0;
            decimal runRevenue = 0m;

            for (int m = 1; m <= 12; m++)
            {
                runUsers += userMonths.GetValueOrDefault(m, 0);
                var oCount = orderData.Orders.Count(o => o.CreatedAt.Month == m);
                var rCount = orderData.Orders.Where(o => o.PaidAt.HasValue && o.PaidAt.Value.Month == m && (o.Status == "Paid" || o.Status == "Completed")).Sum(o => o.TotalAmount);
                runOrders += oCount;
                runRevenue += rCount;

                dataPoints.Add(new DashboardChartPointDto
                {
                    Label = $"{m:00}/{year}",
                    Orders = orderData.BaseOrders + runOrders,
                    Revenue = orderData.BaseRevenue + runRevenue,
                    Users = baseUsers + runUsers,
                    Sellers = 0
                });
            }
        }
        else if (range.Equals("Year", StringComparison.OrdinalIgnoreCase))
        {
            range = "Year";
            var endYear = now.Year;
            var startYear = endYear - 4;
            start = new DateTime(startYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            end = new DateTime(endYear + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var baseUsers = await _db.Users.AsNoTracking().CountAsync(u => u.CreatedAt < start);
            var userYears = await _db.Users.AsNoTracking()
                .Where(u => u.CreatedAt >= start && u.CreatedAt < end)
                .GroupBy(u => u.CreatedAt.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Year, g => g.Count);

            var orderData = await FetchOrdersForChart(start, end);

            int runUsers = 0;
            int runOrders = 0;
            decimal runRevenue = 0m;

            for (int y = startYear; y <= endYear; y++)
            {
                runUsers += userYears.GetValueOrDefault(y, 0);
                var oCount = orderData.Orders.Count(o => o.CreatedAt.Year == y);
                var rCount = orderData.Orders.Where(o => o.PaidAt.HasValue && o.PaidAt.Value.Year == y && (o.Status == "Paid" || o.Status == "Completed")).Sum(o => o.TotalAmount);
                runOrders += oCount;
                runRevenue += rCount;

                dataPoints.Add(new DashboardChartPointDto
                {
                    Label = y.ToString(),
                    Orders = orderData.BaseOrders + runOrders,
                    Revenue = orderData.BaseRevenue + runRevenue,
                    Users = baseUsers + runUsers,
                    Sellers = 0
                });
            }
        }
        else
        {
            // Default: Week (7 ngày gần nhất)
            range = "Week";
            start = now.Date.AddDays(-6);
            end = now.Date.AddDays(1);

            var baseUsers = await _db.Users.AsNoTracking().CountAsync(u => u.CreatedAt < start);
            var userDates = await _db.Users.AsNoTracking()
                .Where(u => u.CreatedAt >= start && u.CreatedAt < end)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);

            var orderData = await FetchOrdersForChart(start, end);

            int runUsers = 0;
            int runOrders = 0;
            decimal runRevenue = 0m;

            for (var d = start; d <= now.Date; d = d.AddDays(1))
            {
                runUsers += userDates.GetValueOrDefault(d.Date, 0);
                var oCount = orderData.Orders.Count(o => o.CreatedAt.Date == d.Date);
                var rCount = orderData.Orders.Where(o => o.PaidAt.HasValue && o.PaidAt.Value.Date == d.Date && (o.Status == "Paid" || o.Status == "Completed")).Sum(o => o.TotalAmount);
                runOrders += oCount;
                runRevenue += rCount;

                dataPoints.Add(new DashboardChartPointDto
                {
                    Label = d.ToString("dd/MM"),
                    Orders = orderData.BaseOrders + runOrders,
                    Revenue = orderData.BaseRevenue + runRevenue,
                    Users = baseUsers + runUsers,
                    Sellers = 0
                });
            }
        }

        var chart = new DashboardChartDto
        {
            Range = range,
            Points = dataPoints
        };

        return Ok(ApiResponse<DashboardChartDto>.Success(chart, "Lấy biểu đồ dashboard thành công"));
    }

    private async Task<OrdersForChartDto> FetchOrdersForChart(DateTime from, DateTime to)
    {
        try
        {
            var client = _clientFactory.CreateClient("ordering");
            AddInternalHeader(client);
            var url = $"internal/ordering/stats/orders-for-chart?from={Uri.EscapeDataString(from.ToString("o"))}&to={Uri.EscapeDataString(to.ToString("o"))}";
            var res = await client.GetFromJsonAsync<OrdersForChartDto>(url);
            return res ?? new OrdersForChartDto();
        }
        catch
        {
            return new OrdersForChartDto();
        }
    }

    private record CountDto(int Count);
    private record OrderSummaryDto(int TotalOrdersThisMonth, int TotalOrdersLastMonth, decimal RevenueThisMonth, decimal RevenueLastMonth);
    private class OrdersForChartDto
    {
        public int BaseOrders { get; set; }
        public decimal BaseRevenue { get; set; }
        public List<OrderItemDto> Orders { get; set; } = new();
    }
    private record OrderItemDto(DateTime CreatedAt, DateTime? PaidAt, decimal TotalAmount, string Status);
}

public class AdminDashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalSellersThisMonth { get; set; }
    public int TotalSellersLastMonth { get; set; }
    public int TotalUsersThisMonth { get; set; }
    public int TotalUsersLastMonth { get; set; }
    public int TotalOrdersThisMonth { get; set; }
    public int TotalOrdersLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}

public class DashboardChartDto
{
    public string Range { get; set; } = "Week";
    public List<DashboardChartPointDto> Points { get; set; } = new();
}

public class DashboardChartPointDto
{
    public string Label { get; set; } = "";
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
    public int Users { get; set; }
    public int Sellers { get; set; }
}
