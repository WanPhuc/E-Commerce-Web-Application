using WebBanHang.Migrations;
using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs.Sellers.Revenue;

public class SellerRevenueDto
{
    public decimal TotalRevenueAll { get; set; }
    public decimal TotalRevenueLastMonth { get; set; }
    public int TotalOrdersSuccessAll { get; set; }
    public int TotalOrdersSuccessLastMonth { get; set; }
    public int TotalUsersBuyingAll { get; set; }
    public int TotalUsersBuyingLastMonth { get; set; }

}
public class SellerTopSellingProductsDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public int SoldCount { get; set; }
    public decimal AverageSellingPrice { get; set; }
    public string ImageUrl { get; set; } = default!;    
    public double Rating { get; set; }
    public decimal TotalRevenue { get; set; }

}

public class SellerRevenueChartPointDto
{
    public string Label { get; set; } = "";
    public decimal? Revenue { get; set; }
    public int Orders { get; set; }
    public int Users { get; set; }
}
public class SellerRevenueChartDto
{
    public ChartRanger Ranger { get; set; }
    public List<SellerRevenueChartPointDto> Points { get; set; } = new List<SellerRevenueChartPointDto>();
}