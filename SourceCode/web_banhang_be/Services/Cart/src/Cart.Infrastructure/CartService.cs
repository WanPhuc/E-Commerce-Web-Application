namespace AuraMart.Cart.Infrastructure;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly CartDbContext _db;

    public CartService(ICartRepository cartRepo, CartDbContext db)
    {
        _cartRepo = cartRepo;
        _db = db;
    }

    public async Task<ApiResponse<CartDto>> GetCartAsync(Guid userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return ApiResponse<CartDto>.Success(MapToDto(cart));
    }

    public async Task<ApiResponse<CartDto>> AddItemAsync(Guid userId, AddToCartRequest request)
    {
        if (request.Quantity <= 0)
        {
            return ApiResponse<CartDto>.Fail("Số lượng sản phẩm phải lớn hơn 0", 400);
        }

        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse<CartDto>.Success(MapToDto(cart), "Đã thêm vào giỏ hàng");
    }

    public async Task<ApiResponse<CartDto>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            return await RemoveItemAsync(userId, cartItemId);
        }

        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);

        if (item == null)
        {
            return ApiResponse<CartDto>.Fail("Không tìm thấy sản phẩm trong giỏ hàng", 404);
        }

        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse<CartDto>.Success(MapToDto(cart), "Cập nhật giỏ hàng thành công");
    }

    public async Task<ApiResponse<CartDto>> RemoveItemAsync(Guid userId, Guid cartItemId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);

        if (item != null)
        {
            cart.CartItems.Remove(item);
            _db.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return ApiResponse<CartDto>.Success(MapToDto(cart), "Đã xóa sản phẩm khỏi giỏ hàng");
    }

    public async Task<ApiResponse<bool>> ClearCartAsync(Guid userId)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart != null)
        {
            _db.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return ApiResponse<bool>.Success(true, "Đã làm trống giỏ hàng");
    }

    private async Task<Domain.Cart> GetOrCreateCartAsync(Guid userId)
    {
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Domain.Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
        }
        return cart;
    }

    private static CartDto MapToDto(Domain.Cart c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        Items = c.CartItems.Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList()
    };
}
