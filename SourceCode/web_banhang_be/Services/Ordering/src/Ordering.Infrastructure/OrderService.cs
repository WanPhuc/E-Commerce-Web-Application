using BuildingBlocks.Outbox;
using BuildingBlocks.EventBus;
using AuraMart.Ordering.Contracts;
using System.Text.Json;

namespace AuraMart.Ordering.Infrastructure;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderingDbContext _db;

    public OrderService(IOrderRepository orderRepository, OrderingDbContext db)
    {
        _orderRepository = orderRepository;
        _db = db;
    }

    public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(Guid userId, string userName, string userEmail, CreateOrderRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return ApiResponse<OrderResponseDto>.Fail("Đơn hàng phải có ít nhất 1 sản phẩm", 400);
        }

        var totalAmount = request.Items.Sum(i => i.Price * i.Quantity);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomerName = userName,
            CustomerEmail = userEmail,
            AddressId = request.AddressId,
            SellerId = request.SellerId,
            TotalAmount = totalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ShippingAddress = new OrderShippingAddress
            {
                RecipientName = request.ShippingAddress.RecipientName,
                PhoneNumber = request.ShippingAddress.PhoneNumber,
                AddressLine = request.ShippingAddress.AddressLine,
                Ward = request.ShippingAddress.Ward,
                District = request.ShippingAddress.District,
                City = request.ShippingAddress.City
            },
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Sku = i.Sku,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        _db.Orders.Add(order);

        // Outbox event
        var orderCreatedEvt = new OrderCreatedIntegrationEvent
        {
            OrderId = order.Id,
            BuyerId = userId,
            SellerId = request.SellerId,
            TotalAmount = totalAmount,
            Items = order.Items.Select(i => new OrderItemEventLine(i.ProductId, i.Quantity)).ToList()
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = orderCreatedEvt.EventId,
            Type = orderCreatedEvt.GetType().AssemblyQualifiedName ?? orderCreatedEvt.GetType().Name,
            Payload = JsonSerializer.Serialize(orderCreatedEvt),
            OccurredOnUtc = orderCreatedEvt.OccurredOnUtc
        });

        await _db.SaveChangesAsync();

        return ApiResponse<OrderResponseDto>.Success(MapToDto(order), "Tạo đơn hàng thành công", 201);
    }

    public async Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(Guid orderId, Guid userId)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
        if (order == null)
        {
            return ApiResponse<OrderResponseDto?>.Fail("Không tìm thấy đơn hàng", 404);
        }

        if (order.UserId != userId && order.SellerId != userId)
        {
            return ApiResponse<OrderResponseDto?>.Fail("Không có quyền xem đơn hàng này", 403);
        }

        return ApiResponse<OrderResponseDto?>.Success(MapToDto(order));
    }

    public async Task<ApiResponse<List<OrderResponseDto>>> GetMyOrdersAsync(Guid userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        var dtos = orders.Select(MapToDto).ToList();
        return ApiResponse<List<OrderResponseDto>>.Success(dtos);
    }

    public async Task<ApiResponse<List<OrderResponseDto>>> GetSellerOrdersAsync(Guid sellerId)
    {
        var orders = await _orderRepository.GetOrdersBySellerIdAsync(sellerId);
        var dtos = orders.Select(MapToDto).ToList();
        return ApiResponse<List<OrderResponseDto>>.Success(dtos);
    }

    public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId, Guid userId, string reason)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
        if (order == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy đơn hàng", 404);
        }

        if (order.UserId != userId && order.SellerId != userId)
        {
            return ApiResponse<bool>.Fail("Không có quyền hủy đơn hàng này", 403);
        }

        if (order.Status == "Completed" || order.Status == "Cancelled")
        {
            return ApiResponse<bool>.Fail($"Không thể hủy đơn hàng ở trạng thái {order.Status}", 400);
        }

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;

        // Outbox event
        var cancelEvt = new OrderCancelledIntegrationEvent
        {
            OrderId = order.Id,
            BuyerId = order.UserId,
            SellerId = order.SellerId,
            Reason = reason,
            Items = order.Items.Select(i => new OrderItemEventLine(i.ProductId, i.Quantity)).ToList()
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = cancelEvt.EventId,
            Type = cancelEvt.GetType().AssemblyQualifiedName ?? cancelEvt.GetType().Name,
            Payload = JsonSerializer.Serialize(cancelEvt),
            OccurredOnUtc = cancelEvt.OccurredOnUtc
        });

        await _db.SaveChangesAsync();

        return ApiResponse<bool>.Success(true, "Hủy đơn hàng thành công");
    }

    private static OrderResponseDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        CustomerName = o.CustomerName,
        CustomerEmail = o.CustomerEmail,
        AddressId = o.AddressId,
        TotalAmount = o.TotalAmount,
        Status = o.Status,
        SellerId = o.SellerId,
        CreatedAt = o.CreatedAt,
        PaidAt = o.PaidAt,
        CompletedAt = o.CompletedAt,
        ShippingAddress = o.ShippingAddress == null ? new() : new()
        {
            RecipientName = o.ShippingAddress.RecipientName,
            PhoneNumber = o.ShippingAddress.PhoneNumber,
            AddressLine = o.ShippingAddress.AddressLine,
            Ward = o.ShippingAddress.Ward,
            District = o.ShippingAddress.District,
            City = o.ShippingAddress.City
        },
        Items = o.Items.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Sku = i.Sku,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList()
    };
}
