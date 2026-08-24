using Microsoft.EntityFrameworkCore;
using WebBanHang.Helpers;
using WebBanHang.Models.EntityModels;
using WebBanHang.Models.Enums;

namespace WebBanHang.Data.Seeders;

public static class DemoDataSeeder
{
    private const string SellerEmail = "seller.demo@shoppy.local";
    private const string BuyerEmail = "buyer.demo@shoppy.local";

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Products.AnyAsync(p => p.SKU.StartsWith("DEMO-")))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var buyerRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Buyer");
        var sellerRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Seller");

        if (buyerRole == null || sellerRole == null)
        {
            return;
        }

        var buyer = await db.Users.FirstOrDefaultAsync(u => u.Email == BuyerEmail);
        if (buyer == null)
        {
            buyer = new User
            {
                ProviderType = (byte)BaseEnums.UserProviderTypeEnum.Email,
                FullName = "Demo Buyer",
                Email = BuyerEmail,
                PasswordHash = PasswordHelper.HashPassword("demo123456"),
                IsActive = true,
                RoleId = buyerRole.Id,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(buyer);
        }

        var sellerUser = await db.Users.FirstOrDefaultAsync(u => u.Email == SellerEmail);
        if (sellerUser == null)
        {
            sellerUser = new User
            {
                ProviderType = (byte)BaseEnums.UserProviderTypeEnum.Email,
                FullName = "Demo Seller",
                Email = SellerEmail,
                PasswordHash = PasswordHelper.HashPassword("demo123456"),
                IsActive = true,
                RoleId = sellerRole.Id,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(sellerUser);
        }

        await db.SaveChangesAsync();

        var buyerAddress = await db.Addresses.FirstOrDefaultAsync(a => a.UserId == buyer.Id && a.IsDefault);
        if (buyerAddress == null)
        {
            buyerAddress = new Address
            {
                UserId = buyer.Id,
                RecipientName = "Demo Buyer",
                PhoneNumber = "0900000001",
                AddressLine = "12 Nguyen Trai",
                Ward = "Ben Thanh",
                District = "Quan 1",
                City = "Ho Chi Minh",
                IsDefault = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Addresses.Add(buyerAddress);
        }

        var sellerAddress = await db.Addresses.FirstOrDefaultAsync(a => a.UserId == sellerUser.Id && a.IsDefault);
        if (sellerAddress == null)
        {
            sellerAddress = new Address
            {
                UserId = sellerUser.Id,
                RecipientName = "Demo Seller Store",
                PhoneNumber = "0900000002",
                AddressLine = "88 Le Loi",
                Ward = "Ben Nghe",
                District = "Quan 1",
                City = "Ho Chi Minh",
                IsDefault = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Addresses.Add(sellerAddress);
        }

        await db.SaveChangesAsync();

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == sellerUser.Id);
        if (seller == null)
        {
            seller = new Seller
            {
                UserId = sellerUser.Id,
                AddressId = sellerAddress.Id,
                StoreName = "Shoppy Demo Store",
                Description = "Demo store for portfolio data.",
                Status = SellerApplicationStatus.Approved,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Sellers.Add(seller);
        }

        var electronics = await FindOrCreateCategoryAsync(db, "Electronics", now);
        var fashion = await FindOrCreateCategoryAsync(db, "Fashion", now);
        var home = await FindOrCreateCategoryAsync(db, "Home & Living", now);

        await db.SaveChangesAsync();

        var products = new[]
        {
            new DemoProduct("DEMO-KEYBOARD-001", "Mechanical Keyboard K87", "Compact mechanical keyboard for work and gaming.", 1190000m, 8, 42, electronics.Id, "https://images.unsplash.com/photo-1587829741301-dc798b83add3.jpg"),
            new DemoProduct("DEMO-MOUSE-001", "Wireless Mouse M2", "Lightweight wireless mouse with silent clicks.", 399000m, 12, 80, electronics.Id, "https://images.unsplash.com/photo-1527814050087-3793815479db.jpg"),
            new DemoProduct("DEMO-HOODIE-001", "Basic Cotton Hoodie", "Soft cotton hoodie for daily wear.", 459000m, 15, 35, fashion.Id, "https://images.unsplash.com/photo-1556821840-3a63f95609a7.jpg"),
            new DemoProduct("DEMO-LAMP-001", "Minimal Desk Lamp", "Warm desk lamp for home office setup.", 329000m, 5, 24, home.Id, "https://images.unsplash.com/photo-1507473885765-e6ed057f782c.jpg")
        };

        var createdProducts = new List<Product>();
        foreach (var item in products)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.SKU == item.Sku);
            if (product == null)
            {
                product = new Product
                {
                    SellerId = seller.Id,
                    CategoryId = item.CategoryId,
                    SKU = item.Sku,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    DiscountPercent = item.DiscountPercent,
                    SoldCount = 0,
                    Stock = item.Stock,
                    LowStockThreshold = 5,
                    Status = ProductStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                db.Products.Add(product);
            }

            createdProducts.Add(product);
        }

        await db.SaveChangesAsync();

        foreach (var product in createdProducts)
        {
            if (!await db.ProductImages.AnyAsync(i => i.ProductId == product.Id))
            {
                var imageUrl = products.First(p => p.Sku == product.SKU).ImageUrl;
                db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = imageUrl,
                    IsMainImage = true,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }

        var cart = await db.Carts.FirstOrDefaultAsync(c => c.UserId == buyer.Id);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = buyer.Id,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Carts.Add(cart);
            await db.SaveChangesAsync();
        }

        var cartProduct = createdProducts.First();
        if (!await db.CartItems.AnyAsync(i => i.CartId == cart.Id && i.ProductId == cartProduct.Id))
        {
            db.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = cartProduct.Id,
                Quantity = 1,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        if (!await db.Orders.AnyAsync(o => o.UserId == buyer.Id && o.SellerId == seller.Id))
        {
            var orderProduct = createdProducts.First();
            var total = orderProduct.Price * 2;
            var order = new Order
            {
                UserId = buyer.Id,
                SellerId = seller.Id,
                AddressId = buyerAddress.Id,
                TotalAmount = total,
                Status = OrderStatus.Completed,
                PaidAt = now.AddDays(-2),
                CompletedAt = now.AddDays(-1),
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-1)
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            db.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = orderProduct.Id,
                Quantity = 2,
                Price = orderProduct.Price,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-3)
            });

            db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                Method = "COD",
                Amount = total,
                Status = PaymentStatus.Paid,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-2)
            });

            db.ProductReviews.Add(new ProductReview
            {
                UserId = buyer.Id,
                ProductId = orderProduct.Id,
                Rating = 5,
                Comment = "Demo review: product quality is good.",
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1)
            });
        }

        if (!await db.Notifications.AnyAsync(n => n.ReceiverId == sellerUser.Id && n.Title == "Demo order completed"))
        {
            db.Notifications.Add(new Notification
            {
                ReceiverId = sellerUser.Id,
                Title = "Demo order completed",
                Message = "A demo order has been completed successfully.",
                RedirectUrl = "/seller/orders",
                Type = NotificationType.Order,
                IsRead = false,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task<Category> FindOrCreateCategoryAsync(AppDbContext db, string name, DateTime now)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (category != null)
        {
            return category;
        }

        category = new Category
        {
            Name = name,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.Categories.Add(category);
        return category;
    }

    private sealed record DemoProduct(
        string Sku,
        string Name,
        string Description,
        decimal Price,
        double DiscountPercent,
        int Stock,
        Guid CategoryId,
        string ImageUrl);
}
