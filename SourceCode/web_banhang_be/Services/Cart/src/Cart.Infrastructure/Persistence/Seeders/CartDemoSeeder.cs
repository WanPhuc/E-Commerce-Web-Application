using Microsoft.EntityFrameworkCore;
using AuraMart.Shared.Constants;

namespace AuraMart.Cart.Infrastructure.Persistence.Seeders;

public static class CartDemoSeeder
{
    public static async Task SeedAsync(CartDbContext context)
    {
        var now = DateTime.UtcNow;

        // Cart 1: Buyer 1
        if (!await context.Carts.AnyAsync(c => c.Id == DemoIds.BuyerCartId))
        {
            context.Carts.Add(new Domain.Cart
            {
                Id = DemoIds.BuyerCartId,
                UserId = DemoIds.BuyerUserId,
                CreatedAt = now,
                CartItems =
                [
                    new Domain.CartItem
                    {
                        Id = DemoIds.BuyerCartItemId,
                        ProductId = DemoIds.TshirtProductId,
                        Quantity = 2,
                        CreatedAt = now
                    },
                    new Domain.CartItem
                    {
                        Id = DemoIds.BuyerCartItemId2,
                        ProductId = DemoIds.MouseProductId,
                        Quantity = 1,
                        CreatedAt = now
                    }
                ]
            });
            await context.SaveChangesAsync();
        }

        // Cart 2: Buyer 2
        if (!await context.Carts.AnyAsync(c => c.Id == DemoIds.Buyer2CartId))
        {
            context.Carts.Add(new Domain.Cart
            {
                Id = DemoIds.Buyer2CartId,
                UserId = DemoIds.Buyer2UserId,
                CreatedAt = now,
                CartItems =
                [
                    new Domain.CartItem
                    {
                        Id = DemoIds.Buyer2CartItemId1,
                        ProductId = DemoIds.KeyboardProductId,
                        Quantity = 1,
                        CreatedAt = now
                    }
                ]
            });
            await context.SaveChangesAsync();
        }
    }
}
