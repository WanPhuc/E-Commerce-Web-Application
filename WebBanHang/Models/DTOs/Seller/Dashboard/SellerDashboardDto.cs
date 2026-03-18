using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs.Sellers;
public class SellerDashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalOrdersToDay { get; set; }
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalRevenueLastMonth { get; set; }
    public decimal TotalRevenueToDay { get; set; }
    public double RevenueGrowth { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public List<TopSellingProductDashboardDto> TopSellingProducts { get; set; } = new List<TopSellingProductDashboardDto>();
    public List<OrdersProcessDashboardDto> OrdersProcess { get; set; } = new List<OrdersProcessDashboardDto>();
    public List<ErrorInventoryDashboardDto> ErrorInventory { get; set; } = new List<ErrorInventoryDashboardDto>();
    public List<RecentRatingDashboardDto> RecentRatings { get; set; } = new List<RecentRatingDashboardDto>();
}   
public class TopSellingProductDashboardDto
{
    public string ProductName { get; set; } = "";
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
public class OrdersProcessDashboardDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustumerName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
public class ErrorInventoryDashboardDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int StockQuantity { get; set; }
    
}
public class RecentRatingDashboardDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public double Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime ReviewDate { get; set; }
}
public class SellerDashboardChartPointDto
{
    public string Label { get; set; } = "";
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
}
public class SellerDashboardChartDto
{
    public ChartRanger Ranger { get; set; }
    public List<SellerDashboardChartPointDto> Points { get; set; } = new List<SellerDashboardChartPointDto>();
}