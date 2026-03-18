using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Migrations;
using WebBanHang.Models;
using WebBanHang.Models.Enums;
public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    public AdminService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<AdminDashboardDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);

        var totalProducts = await _db.Products.AsNoTracking().CountAsync(ct);

        var sellersAll = await _db.Sellers.AsNoTracking()
            .CountAsync(ct);

        var sellersLastMonth = await _db.Sellers.AsNoTracking()
            .CountAsync(x => x.CreatedAt < thisMonthStart, ct);

        var usersAll = await _db.Users.AsNoTracking()
            .Where(u=>u.Role.Name=="User"||u.Role.Name=="Seller")
            .CountAsync(ct);

        var usersLastMonth = await _db.Users.AsNoTracking()
            .Where(u=>u.Role.Name=="User"||u.Role.Name=="Seller")
            .CountAsync(x => x.CreatedAt < thisMonthStart, ct);

        var ordersAll = await _db.Orders.AsNoTracking()
            .CountAsync(ct);

        var ordersLastMonth = await _db.Orders.AsNoTracking()
            .CountAsync(x => x.CreatedAt < thisMonthStart, ct);

        var revenueAll = await _db.Orders.AsNoTracking()
            .Where(o => o.PaidAt != null && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed))
            .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;

        var revenueLastMonth = await _db.Orders.AsNoTracking()
            .Where(o => o.PaidAt != null
                && o.PaidAt < thisMonthStart
                && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed))
            .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;

        return new AdminDashboardDto
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
    }
    public async Task<DashboardChartDto> GetChartAsync(DashboardRanger range, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var dataPoints = new List<DashboardChartPoinDto>();

        if(range == DashboardRanger.Week)
        {
            var start = now.Date.AddDays(-6);
            var endExclusive = now.Date.AddDays(1);

            //BaseLine (all time before start date)
            var baseOrders = await _db.Orders.AsNoTracking()
                .CountAsync(o=>o.CreatedAt < start,ct);
            var baseRevenue = await _db.Orders.AsNoTracking()
                .Where(o=>o.PaidAt !=null&&o.PaidAt<start && (o.Status==OrderStatus.Paid||o.Status==OrderStatus.Completed))
                .SumAsync(o=> (decimal?)o.TotalAmount,ct) ??0m;
            var baseUsers = await _db.Users.AsNoTracking().CountAsync(u=>u.CreatedAt < start && (u.Role.Name=="User"||u.Role.Name=="Seller"),ct);
            var baseSellers = await _db.Sellers.AsNoTracking().CountAsync(s=>s.CreatedAt < start,ct);

            //increments


            var orders=await _db.Orders.AsNoTracking()
                .Where(o=>o.CreatedAt >=start && o.CreatedAt<endExclusive)
                .GroupBy(o=>o.CreatedAt.Date)
                .Select(g=>new{g.Key,Value = g.Count()})
                .ToListAsync(ct);

            var revenue =await _db.Orders.AsNoTracking()
                .Where(o=>o.PaidAt !=null &&o.PaidAt>=start && o.PaidAt<endExclusive&&(o.Status==OrderStatus.Paid||o.Status==OrderStatus.Completed))
                .GroupBy(o=>o.PaidAt!.Value.Date)
                .Select(g=>new {g.Key,value=g.Sum(o=>(decimal?)o.TotalAmount)??0m})
                .ToListAsync(ct);

            var users=await _db.Users.AsNoTracking()
                .Where(u=>u.CreatedAt>=start&&u.CreatedAt<endExclusive&&(u.Role.Name=="User"||u.Role.Name=="Seller"))
                .GroupBy(u=>u.CreatedAt.Date)   
                .Select(g=>new{g.Key ,Value=g.Count()})
                .ToListAsync(ct);

            var seller =await _db.Sellers.AsNoTracking()
                .Where(s=>s.CreatedAt>=start&&s.CreatedAt<endExclusive)
                .GroupBy(s=>s.CreatedAt.Date)
                .Select(g=>new{g.Key,Value=g.Count()})
                .ToListAsync(ct);

            //dict
            var oDict = orders.ToDictionary(o=>o.Key,o=>o.Value);
            var rDict = revenue.ToDictionary(r=>r.Key,r=>r.value);
            var uDict = users.ToDictionary(u=>u.Key,u=>u.Value);
            var sDict = seller.ToDictionary(s=>s.Key,s=>s.Value);

            int runO=0,runU=0,runS=0;
            decimal runR=0m;
            for(var d=start;d<=now.Date;d=d.AddDays(1))
            {
                runO += oDict.TryGetValue(d, out var oi) ? oi : 0;
                runR += rDict.TryGetValue(d, out var ri) ? ri : 0m;
                runU += uDict.TryGetValue(d, out var ui) ? ui : 0;
                runS += sDict.TryGetValue(d, out var si) ? si : 0;
                dataPoints.Add(new DashboardChartPoinDto
                {
                    Label = d.ToString("dd/MM"),
                    Orders = baseOrders + runO,
                    Revenue = baseRevenue + runR,
                    Users = baseUsers + runU,
                    Sellers = baseSellers + runS

                });
            }           
        }
        else if(range == DashboardRanger.Month)
        {
            var year = now.Year;
            var yearStart = new DateTime(year,1,1);
            var nextYearStart = yearStart.AddYears(1);

            // BASELINE: ALL TIME trước 1/1 năm nay
            var baseOrders = await _db.Orders.AsNoTracking().CountAsync(o => o.CreatedAt < yearStart, ct);
            var baseRevenue = await _db.Orders.AsNoTracking()
                .Where(o => o.PaidAt != null
                    && o.PaidAt < yearStart
                    && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed))
                .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;

            var baseUsers = await _db.Users.AsNoTracking()
                .CountAsync(u => u.CreatedAt < yearStart && (u.Role.Name == "User" || u.Role.Name == "Seller"), ct);

            var baseSellers = await _db.Sellers.AsNoTracking()
                .CountAsync(s => s.CreatedAt < yearStart, ct);

            // increments trong năm

            var orders = await _db.Orders.AsNoTracking()
                .Where(o=>o.CreatedAt >=yearStart && o.CreatedAt<nextYearStart)
                .GroupBy(o=>o.CreatedAt.Month)
                .Select(g=>new{Month=g.Key,Value = g.Count()})
                .ToListAsync(ct);
            var revenue = await _db.Orders.AsNoTracking()
                .Where(o=>o.PaidAt !=null && o.PaidAt>=yearStart && o.PaidAt<nextYearStart &&(o.Status==OrderStatus.Paid||o.Status==OrderStatus.Completed))
                .GroupBy(o=>o.PaidAt!.Value.Month)
                .Select(g=>new {Month=g.Key,Value=g.Sum(o=>(decimal?)o.TotalAmount)??0m})
                .ToListAsync(ct);
            var users = await _db.Users.AsNoTracking()
                .Where(u=>u.CreatedAt>=yearStart && u.CreatedAt<nextYearStart&&(u.Role.Name=="User"||u.Role.Name=="Seller"))
                .GroupBy(u=>u.CreatedAt.Month)
                .Select(g=>new{Month=g.Key ,Value=g.Count()})
                .ToListAsync(ct);
            var sellers = await _db.Sellers.AsNoTracking()
                .Where(s=>s.CreatedAt>=yearStart && s.CreatedAt<nextYearStart)
                .GroupBy(s=>s.CreatedAt.Month)
                .Select(g=>new{Month=g.Key,Value=g.Count()})
                .ToListAsync(ct);


            var oDict = orders.ToDictionary(x => x.Month, x => x.Value);
            var rDict = revenue.ToDictionary(x => x.Month, x => x.Value);
            var uDict = users.ToDictionary(x => x.Month, x => x.Value);
            var sDict = sellers.ToDictionary(x => x.Month, x => x.Value);

            int runO = 0, runU = 0, runS = 0;
            decimal runR = 0m;

            for (int m = 1; m <= 12; m++)
            {  
                runO += oDict.TryGetValue(m, out var oi) ? oi : 0;
                runR += rDict.TryGetValue(m, out var ri) ? ri : 0m;
                runU += uDict.TryGetValue(m, out var ui) ? ui : 0;
                runS += sDict.TryGetValue(m, out var si) ? si : 0;
                dataPoints.Add(new DashboardChartPoinDto
                {
                    Label = $"{m:00}/{year}",
                    Orders = baseOrders + runO,
                    Revenue = baseRevenue + runR,
                    Users = baseUsers + runU,
                    Sellers = baseSellers + runS
                });
            }
        }
        else
        {
            var endYear= now.Year;
            var startYear = endYear - 4;

            var startUtc = new DateTime(startYear,1,1);
            var endUtc = new DateTime(endYear + 1,1,1);

            //BaseLine

            var baseOrders = await _db.Orders.AsNoTracking()
                .CountAsync(o=>o.CreatedAt < startUtc,ct);
            var baseRevenue = await _db.Orders.AsNoTracking()
                .Where(o=>o.PaidAt !=null && o.PaidAt<startUtc && (o.Status==OrderStatus.Paid||o.Status==OrderStatus.Completed))
                .SumAsync(o=> (decimal?)o.TotalAmount,ct) ??0m;
            var baseUsers = await _db.Users.AsNoTracking()
                .CountAsync(u=>u.CreatedAt < startUtc && (u.Role.Name=="User"||u.Role.Name=="Seller"),ct);
            var baseSellers = await _db.Sellers.AsNoTracking()
                .CountAsync(s=>s.CreatedAt < startUtc,ct);

            //increments

            var orders = await _db.Orders.AsNoTracking()
                .Where(o=>o.CreatedAt >=startUtc && o.CreatedAt<endUtc)
                .GroupBy(o=>o.CreatedAt.Year)
                .Select(g=>new{Year=g.Key,Value = g.Count()})
                .ToListAsync(ct);
            var revenue = await _db.Orders.AsNoTracking()
                .Where(o=>o.PaidAt !=null && o.PaidAt>=startUtc && o.PaidAt<endUtc &&(o.Status==OrderStatus.Paid||o.Status==OrderStatus.Completed))
                .GroupBy(o=>o.PaidAt!.Value.Year)
                .Select(g=>new{Year=g.Key,Value=g.Sum(o=>(decimal?)o.TotalAmount)??0m})
                .ToListAsync(ct);
            var users = await _db.Users.AsNoTracking()
                .Where(u=>u.CreatedAt >=startUtc && u.CreatedAt<endUtc &&(u.Role.Name=="User"||u.Role.Name=="Seller"))
                .GroupBy(u=>u.CreatedAt.Year)
                .Select(g=>new{Year=g.Key,Value=g.Count()})
                .ToListAsync(ct);
            var sellers = await _db.Sellers.AsNoTracking()
                .Where(s=>s.CreatedAt >=startUtc && s.CreatedAt<endUtc)
                .GroupBy(s=>s.CreatedAt.Year)
                .Select(g=>new{Year=g.Key,Value=g.Count()})
                .ToListAsync(ct);

            //dict
            var oDict = orders.ToDictionary(x => x.Year, x => x.Value);
            var rDict = revenue.ToDictionary(x => x.Year, x => x.Value);
            var uDict = users.ToDictionary(x => x.Year, x => x.Value);
            var sDict = sellers.ToDictionary(x => x.Year, x => x.Value);

            int runO=0,runU=0,runS=0;
            decimal runR=0m;

            for (int i = startYear; i <= endYear; i++)
            {
                runO += oDict.TryGetValue(i,out var oi) ? oi : 0;
                runR += rDict.TryGetValue(i,out var ri) ? ri : 0m;
                runU += uDict.TryGetValue(i,out var ui) ? ui : 0;
                runS += sDict.TryGetValue(i,out var si) ? si : 0;
                dataPoints.Add(new DashboardChartPoinDto
                {
                    Label=i.ToString(),
                    Orders=baseOrders + runO,
                    Revenue=baseRevenue + runR,
                    Users=baseUsers + runU,
                    Sellers=baseSellers + runS
                });
            }

        }
        return new DashboardChartDto
        {
            Ranger=range,
            Points=dataPoints
        };
        
    }


}