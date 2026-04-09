using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Migrations;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class SellerDashboardService : ISellerDashboardService
{
    private readonly AppDbContext _db;
    public SellerDashboardService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<SellerDashboardDto> GetSellerDashboardAsync(Guid sellerId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var toDay = now.Date;

        var totalProducts = await _db.Products.AsNoTracking().Where(p => p.SellerId == sellerId).CountAsync(ct);
        var totalOrdersToDay = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.CreatedAt >= toDay).CountAsync(ct);

        var totalRevenueThisMonth = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.Status == OrderStatus.Completed && o.CompletedAt >= thisMonthStart).SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

        var totalRevenueLastMonth = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt >= lastMonthStart && o.CompletedAt < thisMonthStart && o.Status == OrderStatus.Completed).SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

        var totalRevenueToDay = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt >= toDay && o.Status == OrderStatus.Completed).SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0;

        double revenueGrowth = 0;
        if (totalRevenueLastMonth > 0)
        {
            revenueGrowth = (double)(totalRevenueThisMonth - totalRevenueLastMonth) / (double)totalRevenueLastMonth * 100;
        }
        else
        {
            revenueGrowth = totalRevenueThisMonth > 0 ? 100 : 0;
        }

        var averageRating = await _db.ProductReviews.AsNoTracking().Where(r => r.Product.SellerId == sellerId).AverageAsync(r => (double?)r.Rating, ct) ?? 0;
        var totalReviews = await _db.ProductReviews.AsNoTracking().Where(r => r.Product.SellerId == sellerId).CountAsync(ct);
        var pendingOrders = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Pending).CountAsync(ct);
        var processingOrders = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Processing).CountAsync(ct);
        var shippedOrders = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Shipped || o.Status == OrderStatus.Shipping).CountAsync(ct);
        var cancelledOrders = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.Status == OrderStatus.Cancelled).CountAsync(ct);

        var topSellingProducts = await _db.OrderItems.AsNoTracking()
            .Where(oi => oi.Order.SellerId == sellerId && oi.Order.PaidAt != null && oi.Order.CompletedAt != null && oi.Order.Status == OrderStatus.Completed)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.SKU })
            .Select(g => new TopSellingProductDashboardDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.Quantity * oi.Price),
                ImageUrl = _db.ProductImages.Where(p => p.ProductId == g.Key.ProductId).OrderByDescending(p => p.IsMainImage).Select(p => p.ImageUrl).FirstOrDefault(),
                SKU = g.Key.SKU
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(6)
            .ToListAsync(ct);

        var ordersProcess = await _db.Orders.AsNoTracking()
            .Where(o => o.SellerId == sellerId && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing|| o.Status==OrderStatus.Paid))
            .OrderByDescending(o=>o.CreatedAt)
            .Select(o => new OrdersProcessDashboardDto
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAt,
                CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.Payment != null ? o.Payment.Status : PaymentStatus.Pending

            })
            .Take(5)
            .ToListAsync(ct);

        var errorInventory = await _db.Products.AsNoTracking()
            .Where(p => p.SellerId == sellerId && p.Stock <= p.LowStockThreshold)
            .OrderBy(p => p.Stock)
            .Select(p => new ErrorInventoryDashboardDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                StockQuantity = p.Stock,
                SKU = p.SKU
            })
            .Take(5)
            .ToListAsync(ct);

        var recentRatings = await _db.ProductReviews.AsNoTracking()
            .Where(r => r.Product.SellerId == sellerId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RecentRatingDashboardDto
            {
                ReviewId = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                CustomerName = r.User.FullName,
                SKU = r.Product.SKU,
                ImageUrl = _db.ProductImages.Where(p => p.ProductId == r.ProductId).OrderByDescending(p => p.IsMainImage).Select(p => p.ImageUrl).FirstOrDefault(),
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                ReviewDate = r.CreatedAt
            })
            .Take(5)
            .ToListAsync(ct);

        return new SellerDashboardDto
        {
            TotalProducts = totalProducts,
            TotalOrdersToDay = totalOrdersToDay,
            TotalRevenueThisMonth = totalRevenueThisMonth,
            TotalRevenueLastMonth = totalRevenueLastMonth,
            TotalRevenueToDay = totalRevenueToDay,
            RevenueGrowth = revenueGrowth,
            AverageRating = averageRating,
            TotalReviews = totalReviews,
            PendingOrders = pendingOrders,
            ProcessingOrders = processingOrders,
            ShippedOrders = shippedOrders,
            CancelledOrders = cancelledOrders,
            TopSellingProducts = topSellingProducts,
            OrdersProcess = ordersProcess,
            ErrorInventory = errorInventory,
            RecentRatings = recentRatings
        };

    }
    public async Task<SellerDashboardChartDto> GetSellerDashboardChartAsync(Guid sellerId, ChartRanger ranger, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var dataPoints = new List<SellerDashboardChartPointDto>();

        DateTime start;
        DateTime end = now.Date.AddDays(1);

        if (ranger == ChartRanger.Week)
            start = now.Date.AddDays(-6);
        else if (ranger == ChartRanger.Month)
            start = new DateTime(now.Year, now.Month, 1);
        else if (ranger == ChartRanger.Year)
            start = new DateTime(now.Year, 1, 1);
        else
            start = now.Date.AddDays(-6);

        var daily = await _db.Orders.AsNoTracking()
            .Where(o => o.SellerId == sellerId
                    && o.PaidAt != null
                    && o.CompletedAt != null
                    && o.Status == OrderStatus.Completed
                    && o.CompletedAt >= start
                    && o.CompletedAt < end)
            .GroupBy(o => o.CompletedAt!.Value.Date)
            .Select(g => new
            {
                date = g.Key,
                orders = g.Count(),
                revenue = g.Sum(o => (decimal?)o.TotalAmount) ?? 0m
            })
            .ToListAsync(ct);

        if (ranger == ChartRanger.Year)
        {
            // Group theo tháng, mỗi điểm = data của tháng đó
            var oDict = daily.GroupBy(r => r.date.Month)
                            .ToDictionary(g => g.Key, g => g.Sum(r => r.orders));
            var rDict = daily.GroupBy(r => r.date.Month)
                            .ToDictionary(g => g.Key, g => g.Sum(r => r.revenue));

            for (int m = 1; m <= now.Month; m++)
            {
                dataPoints.Add(new SellerDashboardChartPointDto
                {
                    Label = $"Tháng {m}",
                    Orders = oDict.GetValueOrDefault(m),    // chỉ tháng đó
                    Revenue = rDict.GetValueOrDefault(m)    // chỉ tháng đó
                });
            }
        }
        else
        {
            // Week / Month: mỗi điểm = data của ngày đó
            var oDict = daily.ToDictionary(r => r.date, r => r.orders);
            var rDict = daily.ToDictionary(r => r.date, r => r.revenue);

            for (var date = start; date <= now.Date; date = date.AddDays(1))
            {
                dataPoints.Add(new SellerDashboardChartPointDto
                {
                    Label = date.ToString("dd/MM"),
                    Orders = oDict.GetValueOrDefault(date),   // chỉ ngày đó
                    Revenue = rDict.GetValueOrDefault(date)   // chỉ ngày đó
                });
            }
        }

        return new SellerDashboardChartDto
        {
            Ranger = ranger,
            Points = dataPoints
        };
    }
}
