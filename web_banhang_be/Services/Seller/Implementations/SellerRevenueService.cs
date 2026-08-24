using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Extensions;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers.Revenue;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class SellerRevenueService : ISellerRevenueService
{
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _db;
    private readonly ICommonService _commonService;
    private readonly ISellerRepository _sellerRepository;
    public SellerRevenueService(IProductRepository productRepository, AppDbContext db, ICommonService commonService, ISellerRepository sellerRepository)
    {
        _productRepository = productRepository;
        _db = db;
        _commonService = commonService;
        _sellerRepository = sellerRepository;
    }

    public async Task<ApiResponse<SellerRevenueDto>> GetSellerRevenueAsync(CancellationToken ct = default)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false);
        if (seller == null)
            return ApiResponse<SellerRevenueDto>.Fail("Seller not found", 404, ErrorCodes.Seller.NotFound);
        var sellerId = seller.Id;
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);


        var totalRevenueAll = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && (o.Status == OrderStatus.Completed)).SumAsync(o => (decimal?)o.TotalAmount, ct);
        var totalRevenueLastMonth = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && (o.Status == OrderStatus.Completed) && o.PaidAt < thisMonthStart).SumAsync(o => (decimal?)o.TotalAmount, ct);

        var totalOrdersSuccessAll = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && (o.Status == OrderStatus.Completed) && o.PaidAt != null).CountAsync(ct);
        var totalOrdersSuccessLastMonth = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && (o.Status == OrderStatus.Completed) && o.PaidAt != null && o.PaidAt < thisMonthStart).CountAsync(ct);

        var totalUsersBuyingAll = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && (o.Status == OrderStatus.Completed) && o.PaidAt != null).Select(o => o.UserId).Distinct().CountAsync(ct);
        var totalUsersBuyingLastMonth = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && (o.Status == OrderStatus.Completed) && o.PaidAt != null && o.PaidAt < thisMonthStart).Select(o => o.UserId).Distinct().CountAsync(ct);



        var revenue = new SellerRevenueDto
        {
            TotalRevenueAll = totalRevenueAll ?? 0,
            TotalRevenueLastMonth = totalRevenueLastMonth ?? 0,
            TotalOrdersSuccessAll = totalOrdersSuccessAll,
            TotalOrdersSuccessLastMonth = totalOrdersSuccessLastMonth,
            TotalUsersBuyingAll = totalUsersBuyingAll,
            TotalUsersBuyingLastMonth = totalUsersBuyingLastMonth
        };
        return ApiResponse<SellerRevenueDto>.Success(revenue, "Success", 200, SuccessCodes.Seller.RevenueRetrieved);
    }

    public async Task<ApiResponse<SellerRevenueChartDto>> GetSellerRevenueChartAsync(ChartRanger range, CancellationToken ct = default)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false);
        if (seller == null)
            return ApiResponse<SellerRevenueChartDto>.Fail("Seller not found", 404, ErrorCodes.Seller.NotFound);
        var sellerId = seller.Id;

        var now = DateTime.UtcNow;
        var DataPoints = new List<SellerRevenueChartPointDto>();

        DateTime start;
        DateTime end = now.Date.AddDays(1);

        if (range == ChartRanger.Week)
        {
            start = now.Date.AddDays(-6);




        }
        else if (range == ChartRanger.Month)
        {
            start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        else if (range == ChartRanger.Year)
        {
            start = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        else
        {
            start = now.Date.AddDays(-6);
        }
        var userFirstPurchaseData = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.Status == OrderStatus.Completed).GroupBy(o => o.UserId).Select(g => new { UserId = g.Key, FirstArrival = g.Min(o => o.CompletedAt!.Value.Date) }).ToListAsync(ct);

        //baseline
        var baseRevenue = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt < start && (o.Status == OrderStatus.Completed)).SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;
        var baseOrdersSuccess = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt < start && (o.Status == OrderStatus.Completed)).CountAsync(ct);
        var baseUsersBuying = userFirstPurchaseData.Count(u => u.FirstArrival < start);

        //daily data
        var dailyRevenue = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt >= start && o.CompletedAt < end && o.Status == OrderStatus.Completed).GroupBy(o => o.CompletedAt!.Value.Date).Select(g => new { date = g.Key, value = g.Sum(o => (decimal?)o.TotalAmount) ?? 0m }).ToListAsync(ct);
        var dailyOrdersSuccess = await _db.Orders.AsNoTracking().Where(o => o.SellerId == sellerId && o.PaidAt != null && o.CompletedAt != null && o.CompletedAt >= start && o.CompletedAt < end && o.Status == OrderStatus.Completed).GroupBy(o => o.CompletedAt!.Value.Date).Select(g => new { date = g.Key, value = g.Count() }).ToListAsync(ct);


        int runO = 0, runU = 0;
        decimal runR = 0;

        if (range == ChartRanger.Year)
        {
            var rDict = dailyRevenue.GroupBy(r => r.date.Month).ToDictionary(g => g.Key, g => g.Sum(r => r.value));
            var oDict = dailyOrdersSuccess.GroupBy(r => r.date.Month).ToDictionary(g => g.Key, g => g.Sum(r => r.value));
            var uDict = userFirstPurchaseData.Where(u => u.FirstArrival >= start && u.FirstArrival < end).GroupBy(u => u.FirstArrival.Month).ToDictionary(g => g.Key, g => g.Count());

            for (int m = 1; m <= now.Month; m++)
            {
                runR += rDict.GetValueOrDefault(m);
                runO += oDict.GetValueOrDefault(m);
                runU += uDict.GetValueOrDefault(m);
                DataPoints.Add(new SellerRevenueChartPointDto
                {
                    Label = $"Tháng {m}",
                    Revenue = baseRevenue + runR,
                    Orders = baseOrdersSuccess + runO,
                    Users = baseUsersBuying + runU
                });
            }
        }
        else
        {
            // to Dictionary

            var rDict = dailyRevenue.ToDictionary(r => r.date, r => r.value);
            var oDict = dailyOrdersSuccess.ToDictionary(r => r.date, r => r.value);
            var uDict = userFirstPurchaseData.Where(u => u.FirstArrival >= start && u.FirstArrival < end).GroupBy(u => u.FirstArrival).ToDictionary(g => g.Key, g => g.Count());


            for (var date = start; date <= now.Date; date = date.AddDays(1))
            {
                runO += oDict.GetValueOrDefault(date);
                runU += uDict.GetValueOrDefault(date);
                runR += rDict.GetValueOrDefault(date);
                DataPoints.Add(new SellerRevenueChartPointDto
                {
                    Label = date.ToString("dd/MM"),
                    Revenue = baseRevenue + runR,
                    Orders = baseOrdersSuccess + runO,
                    Users = baseUsersBuying + runU
                });
            }
        }
        var chart = new SellerRevenueChartDto
        {
            Ranger = range,
            Points = DataPoints
        };
        return ApiResponse<SellerRevenueChartDto>.Success(chart, "Success", 200, SuccessCodes.Seller.RevenueChartRetrieved);

    }
    public async Task<ApiResponse<PagedResult<SellerTopSellingProductsDto>>> GetSellerTopSellingProductsAsync(PagedRequest request, CancellationToken ct = default)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false);
        if (seller == null)
            return ApiResponse<PagedResult<SellerTopSellingProductsDto>>.Fail("Seller not found.", 404);
        var sellerId = seller.Id;

        var query = _db.OrderItems.AsNoTracking()
            .Where(oi => oi.Product.SellerId == sellerId && oi.Order.PaidAt != null && oi.Order.CompletedAt != null && oi.Order.Status == OrderStatus.Completed)
            .GroupBy(oi => new
            {
                oi.ProductId,
                oi.Product.Name,
                oi.Product.SKU,
                thumbnail = oi.Product.Images.OrderBy(i => i.Id).Select(i => i.ImageUrl).FirstOrDefault()
            })
            .Select(g => new SellerTopSellingProductsDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                SKU = g.Key.SKU,
                ImageUrl = g.Key.thumbnail ?? "",
                Rating = _db.ProductReviews.Where(r => r.ProductId == g.Key.ProductId).Average(r => (double?)r.Rating) ?? 0,
                SoldCount = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => (decimal)oi.Quantity * oi.Price)
            })
            .OrderByDescending(p => p.SoldCount)
            .AsQueryable();

        var topProducts = await query.ToPagedAsync(request);
        foreach (var p in topProducts.Data)
        {
            if (p.SoldCount > 0)
            {
                p.AverageSellingPrice = p.TotalRevenue / p.SoldCount;
            }
        }
        return ApiResponse<PagedResult<SellerTopSellingProductsDto>>.Success(topProducts);
    }


}
