using Microsoft.EntityFrameworkCore;
using AuraMart.Catalog.Infrastructure.Persistence;
using AuraMart.Shared.Constants;
using AuraMart.Catalog.Domain;

namespace AuraMart.Catalog.Infrastructure.Persistence.Seeders;

public static class CatalogDemoSeeder
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (await context.Products.AnyAsync(p => p.Id == DemoIds.IphoneProductId))
            return;

        var now = DateTime.UtcNow;

        // 1. Seed 5 standard categories
        if (!await context.Categories.AnyAsync(c => c.Id == DemoIds.ElectronicsCategoryId))
        {
            context.Categories.AddRange(
                new Category { Id = DemoIds.ElectronicsCategoryId, Name = "Điện Tử & Công Nghệ", CreatedAt = now },
                new Category { Id = DemoIds.FashionCategoryId, Name = "Thời Trang & Phụ Kiện", CreatedAt = now },
                new Category { Id = DemoIds.HomeCategoryId, Name = "Nhà Cửa & Đời Sống", CreatedAt = now },
                new Category { Id = DemoIds.BeautyCategoryId, Name = "Sức Khỏe & Sắc Đẹp", CreatedAt = now },
                new Category { Id = DemoIds.SportsCategoryId, Name = "Thể Thao & Dã Ngoại", CreatedAt = now });
            await context.SaveChangesAsync();
        }

        // 2. Seed 8 rich demo products
        var products = new List<Product>
        {
            // 1. iPhone 15 Pro Max (Seller 1 - AuraMart Official)
            new()
            {
                Id = DemoIds.IphoneProductId,
                Name = "iPhone 15 Pro Max 256GB Titan Tự Nhiên",
                Description = "Siêu phẩm flagship từ Apple với khung viền Titan chuẩn hàng không vũ trụ, chip A17 Pro mạnh mẽ và hệ thống camera tiềm vọng 5x.",
                Price = 29990000,
                DiscountPercent = 5,
                SoldCount = 12,
                Stock = 50,
                LowStockThreshold = 5,
                SKU = "DEMO-IPHONE-001",
                CategoryId = DemoIds.ElectronicsCategoryId,
                SellerId = DemoIds.SellerId,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.IphoneImageId,
                        ProductId = DemoIds.IphoneProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 2. Mechanical Keyboard K87 (Seller 2 - TechZone)
            new()
            {
                Id = DemoIds.KeyboardProductId,
                Name = "Bàn Phím Cơ Không Dây K87 RGB Hot-Swap",
                Description = "Bàn phím cơ layout 87 phím nhỏ gọn, kết nối 3 chế độ (Bluetooth 5.0, 2.4GHz, Type-C), switch Gateron Pro êm ái.",
                Price = 1190000,
                DiscountPercent = 8,
                SoldCount = 26,
                Stock = 42,
                LowStockThreshold = 5,
                SKU = "DEMO-KEYBOARD-001",
                CategoryId = DemoIds.ElectronicsCategoryId,
                SellerId = DemoIds.Seller2Id,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.KeyboardImageId,
                        ProductId = DemoIds.KeyboardProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 3. Wireless Mouse M2 (Seller 2 - TechZone)
            new()
            {
                Id = DemoIds.MouseProductId,
                Name = "Chuột Không Dây Công Thái Học Silent M2",
                Description = "Chuột không dây thiết kế ôm tay chống mỏi cổ tay, phím bấm chống ồn 90%, pin sạc type-C dùng liên tục 60 ngày.",
                Price = 399000,
                DiscountPercent = 12,
                SoldCount = 85,
                Stock = 80,
                LowStockThreshold = 10,
                SKU = "DEMO-MOUSE-001",
                CategoryId = DemoIds.ElectronicsCategoryId,
                SellerId = DemoIds.Seller2Id,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.MouseImageId,
                        ProductId = DemoIds.MouseProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 4. Áo Thun Cotton Nam (Seller 1 - AuraMart Official)
            new()
            {
                Id = DemoIds.TshirtProductId,
                Name = "Áo Thun Cotton Nam Classic Form Rộng",
                Description = "Chất liệu 100% Cotton Compact 2 chiều dày dặn 250gsm, thoáng mát, thấm hút mồ hôi tốt, chống xù lông.",
                Price = 199000,
                DiscountPercent = 10,
                SoldCount = 48,
                Stock = 200,
                LowStockThreshold = 20,
                SKU = "DEMO-TSHIRT-001",
                CategoryId = DemoIds.FashionCategoryId,
                SellerId = DemoIds.SellerId,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.TshirtImageId,
                        ProductId = DemoIds.TshirtProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 5. Basic Cotton Hoodie (Seller 2 - TechZone)
            new()
            {
                Id = DemoIds.HoodieProductId,
                Name = "Áo Khoác Hoodie Unisex Nỉ Bông Streetwear",
                Description = "Áo hoodie nỉ bông cao cấp giữ ấm tối đa, form suông phong cách streetwear trẻ trung năng động.",
                Price = 459000,
                DiscountPercent = 15,
                SoldCount = 35,
                Stock = 65,
                LowStockThreshold = 10,
                SKU = "DEMO-HOODIE-001",
                CategoryId = DemoIds.FashionCategoryId,
                SellerId = DemoIds.Seller2Id,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.HoodieImageId,
                        ProductId = DemoIds.HoodieProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1556905055-8f358a7a47b2?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 6. Nồi Chiên Không Dầu (Seller 1 - AuraMart Official)
            new()
            {
                Id = DemoIds.AirFryerProductId,
                Name = "Nồi Chiên Không Dầu Điện Tử 6.5L",
                Description = "Công nghệ Rapid Air giảm 80% lượng dầu mỡ, bảng điều khiển cảm ứng LED 8 chương trình nấu tự động.",
                Price = 1450000,
                DiscountPercent = 8,
                SoldCount = 9,
                Stock = 80,
                LowStockThreshold = 8,
                SKU = "DEMO-AIRFRYER-001",
                CategoryId = DemoIds.HomeCategoryId,
                SellerId = DemoIds.SellerId,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.AirFryerImageId,
                        ProductId = DemoIds.AirFryerProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1584992236310-6edddc08acff?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 7. Đèn Bàn LED Chống Cận (Seller 1 - AuraMart Official)
            new()
            {
                Id = DemoIds.DeskLampProductId,
                Name = "Đèn Bàn Học LED Chống Cận Thị 3 Chế Độ Sáng",
                Description = "Đèn bàn bảo vệ thị lực chuẩn CRI > 95, điều chỉnh độ sáng cảm ứng mượt mà, tích hợp cổng sạc USB tiện lợi.",
                Price = 329000,
                DiscountPercent = 5,
                SoldCount = 24,
                Stock = 50,
                LowStockThreshold = 10,
                SKU = "DEMO-LAMP-001",
                CategoryId = DemoIds.HomeCategoryId,
                SellerId = DemoIds.SellerId,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.DeskLampImageId,
                        ProductId = DemoIds.DeskLampProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1534349762230-e0cadf78f5da?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            },

            // 8. Bình Giữ Nhiệt Inox 316 (Seller 1 - AuraMart Official)
            new()
            {
                Id = DemoIds.ThermosProductId,
                Name = "Bình Giữ Nhiệt Inox 316 Cao Cấp 750ml",
                Description = "Lõi Inox 316 chuẩn y tế an toàn tuyệt đối, giữ nhiệt nóng 12h và lạnh 24h, kèm nắp hiển thị nhiệt độ cảm ứng.",
                Price = 289000,
                DiscountPercent = 10,
                SoldCount = 53,
                Stock = 90,
                LowStockThreshold = 15,
                SKU = "DEMO-THERMOS-001",
                CategoryId = DemoIds.SportsCategoryId,
                SellerId = DemoIds.SellerId,
                Status = ProductStatus.Active,
                CreatedAt = now,
                Images = new List<ProductImage>
                {
                    new()
                    {
                        Id = DemoIds.ThermosImageId,
                        ProductId = DemoIds.ThermosProductId,
                        ImageUrl = "https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=800",
                        IsMainImage = true,
                        CreatedAt = now
                    }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // 3. Seed Product Reviews
        if (!await context.ProductReviews.AnyAsync(r => r.ProductId == DemoIds.IphoneProductId && r.UserId == DemoIds.BuyerUserId))
        {
            context.ProductReviews.AddRange(
                new ProductReview
                {
                    Id = DemoIds.IphoneReviewId,
                    ProductId = DemoIds.IphoneProductId,
                    UserId = DemoIds.BuyerUserId,
                    DisplayName = "Demo Buyer",
                    Rating = 5,
                    Comment = "Hàng chính hãng nguyên seal, đóng gói kỹ lưỡng và giao cực nhanh!",
                    CreatedAt = now.AddHours(-1)
                },
                new ProductReview
                {
                    Id = DemoIds.KeyboardReviewId,
                    ProductId = DemoIds.KeyboardProductId,
                    UserId = DemoIds.BuyerUserId,
                    DisplayName = "Demo Buyer",
                    Rating = 5,
                    Comment = "Bàn phím gõ rất sướng tay, LED RGB đẹp, kết nối không dây ổn định.",
                    CreatedAt = now.AddHours(-2)
                },
                new ProductReview
                {
                    Id = DemoIds.TshirtReviewId,
                    ProductId = DemoIds.TshirtProductId,
                    UserId = DemoIds.Buyer2UserId,
                    DisplayName = "Demo Buyer 2",
                    Rating = 5,
                    Comment = "Vải cotton dày dặn, mặc thoáng mát, chuẩn form.",
                    CreatedAt = now.AddDays(-1)
                });

            await context.SaveChangesAsync();
        }
    }
}
