using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;
public class SellerOrderService : ISellerOrderService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IOderRepository _orderRepository;

    public SellerOrderService(ISellerRepository sellerRepository, IOderRepository orderRepository)
    {
        _sellerRepository = sellerRepository;
        _orderRepository = orderRepository;
    }

    public async Task<List<SellerOrderDto>> GetSellerOrdersAsync(Guid userId)
    {
        var seller =await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(seller == null) throw new KeyNotFoundException("Seller not found.");

        var orders = await _orderRepository.GetOrdersBySellerAsync(seller.Id);
        if (orders == null || !orders.Any())
        {
            return new List<SellerOrderDto>();
        }
        var sellerOrders= orders.Select(o=> new SellerOrderDto
        {
            Id=o.Id,
            UserName=o.Address?.RecipientName??"N/A",
            PhoneNumber=o.Address?.PhoneNumber??"N/A",
            City=o.Address?.City??"N/A",
            TotalAmount=o.TotalAmount,
            Status=o.Status,
            CreatedAt=o.CreatedAt,
            UpdatedAt=o.UpdatedAt,
            PaidAt=o.PaidAt,
            CompletedAt=o.CompletedAt,
            PaymentMethod=o.Payment?.Method??"N/A",
            PaymentStatus=o.Payment?.Status??PaymentStatus.Pending,
            Items=o.Items.Select(oi=> new SellerOrderItemDto
            {
                ProductId=oi.ProductId,
                ProductName=oi.Product?.Name??"N/A",
                ImageUrl=oi.Product?.Images?.FirstOrDefault(img=>img.IsMainImage)?.ImageUrl??oi.Product?.Images?.FirstOrDefault()?.ImageUrl??"N/A",
                Quantity=oi.Quantity,
                Price=oi.Price
                
            }).ToList()
        }).ToList();
        return sellerOrders;
    }
    public async Task<SellerOrderDto> GetDetailSellerOrderByIdAsync( Guid orderId,Guid userId)
    {
        var seller =await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(seller == null) throw new KeyNotFoundException("Seller not found.");
        var selllerOrder =await _orderRepository.GetOrderDetailsByIdAsync(orderId);
        if(selllerOrder==null || selllerOrder.SellerId!=seller.Id)
        {
            throw new KeyNotFoundException("Order not found or you don't have permission to access this order.");
        }
        var sellerOrderDto = new SellerOrderDto
        {
            Id=selllerOrder.Id,
            UserName=selllerOrder.Address?.RecipientName??"N/A",
            PhoneNumber=selllerOrder.Address?.PhoneNumber??"N/A",
            City=selllerOrder.Address?.City??"N/A",
            TotalAmount=selllerOrder.TotalAmount,
            Status=selllerOrder.Status,
            CreatedAt=selllerOrder.CreatedAt,
            UpdatedAt=selllerOrder.UpdatedAt,
            PaidAt=selllerOrder.PaidAt,
            CompletedAt=selllerOrder.CompletedAt,
            PaymentMethod=selllerOrder.Payment?.Method??"N/A",
            PaymentStatus=selllerOrder.Payment?.Status??PaymentStatus.Pending,
            Items=selllerOrder.Items.Select(oi=> new SellerOrderItemDto
            {
                ProductId=oi.ProductId,
                ProductName=oi.Product?.Name??"N/A",
                ImageUrl=oi.Product?.Images?.FirstOrDefault(img=>img.IsMainImage)?.ImageUrl??oi.Product?.Images?.FirstOrDefault()?.ImageUrl??"N/A",
                Quantity=oi.Quantity,
                Price=oi.Price
                
            }).ToList()
        };
        return sellerOrderDto;
    
    }
    public async Task UpdateSellerOrderStatusAsync(Guid userId, Guid orderId, OrderStatus newStatus)
    {
        var seller =await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(seller == null) throw new KeyNotFoundException("Seller not found.");
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.SellerId != seller.Id)
        {
            throw new KeyNotFoundException("Order not found or you don't have permission to update this order.");
        }
        if(newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("You cannot set order status to Completed or Cancelled directly. Please update the order status to Paid first, then the system will automatically update the order status to Completed when the order is delivered successfully.");
        }
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        if(newStatus == OrderStatus.Paid)
        {
            order.PaidAt = DateTime.UtcNow;
        }
        if(newStatus == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }
        await _orderRepository.UpdateAsync(order);
    }

}