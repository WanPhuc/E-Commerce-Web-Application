using Microsoft.EntityFrameworkCore;
using WebBanHang.Extensions;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class SellerOrderService : ISellerOrderService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IOderRepository _orderRepository;
    private readonly ICommonService _commonService;

    public SellerOrderService(ISellerRepository sellerRepository, IOderRepository orderRepository, ICommonService commonService)
    {
        _sellerRepository = sellerRepository;
        _orderRepository = orderRepository;
        _commonService = commonService;
    }

    public async Task<ApiResponse<PagedResult<SellerOrderDto>>> GetSellerOrdersAsync(PagedRequest request)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false);
        if (seller == null) return ApiResponse<PagedResult<SellerOrderDto>>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var query = _orderRepository.FindByCondition(o => o.SellerId == seller.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new SellerOrderDto
            {
                Id = o.Id,
                UserName = o.User.FullName,
                PhoneNumber = o.Address.PhoneNumber,
                City = o.Address.City,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                PaidAt = o.PaidAt,
                CompletedAt = o.CompletedAt,
                PaymentMethod = o.Payment.Method,
                PaymentStatus = o.Payment.Status,
                Items = o.Items.Select(oi => new SellerOrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ImageUrl = oi.Product.Images
                        .Where(img => img.IsMainImage)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                        ?? oi.Product.Images
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                        ?? "N/A",
                    Quantity = oi.Quantity,
                    Price = oi.Price

                }).ToList()
            });
        var result = await query.ToPagedAsync(request);
        return ApiResponse<PagedResult<SellerOrderDto>>.Success(result, "Success", 200, SuccessCodes.Seller.OrdersRetrieved);
    }
    public async Task<ApiResponse<SellerOrderDto>> GetDetailSellerOrderByIdAsync(Guid orderId)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), false);
        if (seller == null) return ApiResponse<SellerOrderDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var sellerOrderDto = await _orderRepository
            .FindByCondition(o => o.Id == orderId && o.SellerId == seller.Id)
            .Select(o => new SellerOrderDto
            {
                Id = o.Id,
                UserName = o.Address != null ? o.Address.RecipientName : "N/A",
                PhoneNumber = o.Address != null ? o.Address.PhoneNumber : "N/A",
                City = o.Address != null ? o.Address.City : "N/A",
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                PaidAt = o.PaidAt,
                CompletedAt = o.CompletedAt,
                PaymentMethod = o.Payment != null ? o.Payment.Method : "N/A",
                PaymentStatus = o.Payment != null ? o.Payment.Status : PaymentStatus.Pending,
                Items = o.Items.Select(oi => new SellerOrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : "N/A",
                    ImageUrl = oi.Product.Images
                        .Where(img => img.IsMainImage)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                        ?? oi.Product.Images
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                        ?? "N/A",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (sellerOrderDto == null)
        {
            return ApiResponse<SellerOrderDto>.Fail("Order not found or you don't have permission to access this order.", 404, ErrorCodes.Order.PermissionDenied);
        }
        return ApiResponse<SellerOrderDto>.Success(sellerOrderDto, "Success", 200, SuccessCodes.Seller.OrderDetailRetrieved);

    }
    public async Task<ApiResponse<string>> UpdateSellerOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var seller = await _sellerRepository.FindSingleByConditionAsync(s => s.UserId == _commonService.GetUserId(), true);
        if (seller == null) return ApiResponse<string>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.SellerId != seller.Id)
        {
            return ApiResponse<string>.Fail("Order not found or you don't have permission to update this order.", 404, ErrorCodes.Order.PermissionDenied);
        }
        if (newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled)
        {
            return ApiResponse<string>.Fail("You cannot set order status to Completed or Cancelled directly. Please update the order status to Paid first, then the system will automatically update the order status to Completed when the order is delivered successfully.", 400, ErrorCodes.Order.InvalidStatusTransition);
        }
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        if (newStatus == OrderStatus.Paid)
        {
            order.PaidAt = DateTime.UtcNow;
        }
        if (newStatus == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }
        await _orderRepository.UpdateAsync(order);
        return ApiResponse<string>.Success("Order status updated successfully.", "Order status updated successfully.", 200, SuccessCodes.Seller.OrderStatusUpdated);
    }

}
