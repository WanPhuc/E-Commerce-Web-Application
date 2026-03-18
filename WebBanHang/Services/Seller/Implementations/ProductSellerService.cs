using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Helpers.Product;
using WebBanHang.Models;
using WebBanHang.Models.DTOs.Seller.Product;
using WebBanHang.Models.DTOs.Seller.ProductImage;
using WebBanHang.Models.DTOs.Sellers.Product;
using WebBanHang.Repositories;
using WebBanHang.Services.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class ProductSellerService : IProductSellerService
{
    private readonly IProductRepository _productRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly AppDbContext _context;
    private readonly FileHelper _fileHelper;
    public ProductSellerService(IProductRepository productRepository, ISellerRepository sellerRepository, ICategoryRepository categoryRepository, AppDbContext context, FileHelper fileHelper)
    {
        _productRepository = productRepository;
        _sellerRepository = sellerRepository;
        _categoryRepository = categoryRepository;
        _context = context;
        _fileHelper = fileHelper;
    }
    public async Task<List<SellerProductDto>> GetAllProductsAsync(Guid userId)
    {
        var seller =await _sellerRepository.GetSellerByUserIdAsync(userId);
        if (seller == null) throw new KeyNotFoundException("Seller not found.");
        var products = await _productRepository.GetProductsBySellerAsync(seller.Id);
        if (products == null || !products.Any())
        {
            return new List<SellerProductDto>();
        }
        var productDtos = products.Select(p => new SellerProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            SKU = p.SKU,
            Price = p.Price,
            Stock = p.Stock,
            Status = p.Status,
            DiscountPercent = p.DiscountPercent,
            Rating = p.Reviews.Any()? p.Reviews.Average(r => (double?)r.Rating) ?? 0 : 0,
            ReviewCount = p.Reviews.Count,
            SoldCount = p.SoldCount,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            CreatedAt = p.CreatedAt,
            MainImageUrl = p.Images.FirstOrDefault(i => i.IsMainImage == true)?.ImageUrl ?? p.Images.FirstOrDefault()?.ImageUrl,
            

        }).ToList();
        return productDtos;
    }
    public async Task<SellerProductDto> GetProductByIdAsync(Guid productId)
    {
        var productid = await _productRepository.GetProductsDetailsByIdAsync(productId);
        if (productid == null) throw new KeyNotFoundException("Product not found.");
        var productDtos = new SellerProductDto
        {
            Id = productid.Id,
            Name = productid.Name,
            Description = productid.Description,
            SKU = productid.SKU,
            Price = productid.Price,
            Stock = productid.Stock,
            Status = productid.Status,
            DiscountPercent = productid.DiscountPercent,
            Rating = productid.Reviews.Any() ? productid.Reviews.Average(r => (double?)r.Rating) ?? 0 : 0,
            ReviewCount = productid.Reviews.Count,
            SoldCount = productid.SoldCount,
            CategoryId = productid.CategoryId,
            CategoryName = productid.Category.Name,
            CreatedAt = productid.CreatedAt,
            MainImageUrl = productid.Images.FirstOrDefault(i => i.IsMainImage == true)?.ImageUrl ?? productid.Images.FirstOrDefault()?.ImageUrl,
            Images = productid.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsMainImage = i.IsMainImage
            }).ToList()

        };
        return productDtos;
    }
    public async Task<SellerProductDto> CreateProductAsync(Guid userId,CreateSellerProductDto createSellerProductDto)
    {
        var productid=Guid.NewGuid();
        var seller=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(seller==null) throw new KeyNotFoundException("Seller not found.");
        var sku=await _productRepository.GetProductBySKUAsync(createSellerProductDto.SKU);
        if(sku!= null) throw new InvalidOperationException("SKU already exists.");
        var category =await _categoryRepository.GetByIdAsync(createSellerProductDto.CategoryId);
        if(category == null) throw new KeyNotFoundException("Category not found.");
        var productimage= new List<ProductImage>();
        if (createSellerProductDto.Images != null && createSellerProductDto.Images.Any())
        {
            foreach (var imgDto in createSellerProductDto.Images)
            {
                _fileHelper.ValidateImageUrl(imgDto.ImageUrl);

                productimage.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = imgDto.ImageUrl,
                    IsMainImage = imgDto.IsMainImage,
                    ProductId = productid
                });
            }

            if (!productimage.Any(i => i.IsMainImage))
            {
                productimage.First().IsMainImage = true;
            }
            else
            {
                var mainImage = productimage.First(i => i.IsMainImage);
                productimage.ForEach(i => i.IsMainImage = false);
                mainImage.IsMainImage = true;
            }
        }
        var product=new Product
        {
            Id=productid,
            Name=createSellerProductDto.Name,
            Description=createSellerProductDto.Description,
            SKU=createSellerProductDto.SKU,
            Price=createSellerProductDto.Price,
            Stock=createSellerProductDto.Stock,
            Status=createSellerProductDto.Status,
            DiscountPercent=createSellerProductDto.DiscountPercent,
            CategoryId=createSellerProductDto.CategoryId,
            SellerId=seller.Id,
            CreatedAt=DateTime.UtcNow,
            SoldCount=0,
            Images=productimage
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        return new SellerProductDto
        {
            Id=product.Id,
            Name=product.Name,
            Description=product.Description??"",
            SKU=product.SKU,
            Price=product.Price,
            Stock=product.Stock,
            Status=product.Status,
            DiscountPercent=product.DiscountPercent,
            Rating=0,
            ReviewCount=0,
            SoldCount=product.SoldCount,
            CategoryId=product.CategoryId,
            CategoryName=category.Name,
            CreatedAt=product.CreatedAt,
            MainImageUrl=product.Images.FirstOrDefault(i=>i.IsMainImage)?.ImageUrl??product.Images.FirstOrDefault()?.ImageUrl,
            Images=product.Images.Select(i=>new ProductImageDto
            {
                Id=i.Id,
                ImageUrl=i.ImageUrl,
                IsMainImage=i.IsMainImage
            }).ToList()

        };
    }
    public async Task<SellerProductDto> UpdateProductAsync(Guid userId,Guid productId,UpdateSellerProductDto dto)
    {
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products
            .Include(p=>p.Category)
            .Include(p=>p.Images)
            .FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var exitsSKU=await _productRepository.GetProductBySKUAsync(dto.SKU);
        if(exitsSKU != null&& exitsSKU.Id!=productId) throw new InvalidOperationException("SKU already exists.");
        var categry=await _context.Categories.FindAsync(dto.CategoryId);
        if(categry == null) throw new KeyNotFoundException("Category not found.");

        product.Name=dto.Name;
        product.Description=dto.Description;
        product.SKU=dto.SKU;
        product.Price=dto.Price;
        product.Stock=dto.Stock;
        product.Status=dto.Status;
        product.DiscountPercent=dto.DiscountPercent;
        product.CategoryId=dto.CategoryId;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return new SellerProductDto
        {
            Id=product.Id,
            Name=product.Name,
            Description=product.Description??"",
            SKU=product.SKU,
            Price=product.Price,
            Stock=product.Stock,
            Status=product.Status,
            DiscountPercent=product.DiscountPercent,
            Rating=product.Reviews.Any() ? product.Reviews.Average(r => (double?)r.Rating) ?? 0 : 0,
            ReviewCount=product.Reviews.Count,
            SoldCount=product.SoldCount,
            CategoryId=product.CategoryId,
            CategoryName=categry.Name,
            CreatedAt=product.CreatedAt,
            MainImageUrl=product.Images.FirstOrDefault(i=>i.IsMainImage)?.ImageUrl??product.Images.FirstOrDefault()?.ImageUrl,
            Images=product.Images.Select(i=>new ProductImageDto
            {
                Id=i.Id,
                ImageUrl=i.ImageUrl,
                IsMainImage=i.IsMainImage
            }).ToList()

        };
    }
    public async Task DeleteProductAsync(Guid userId,Guid productId)
    {
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products.FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var hasOrders=await _context.OrderItems.FirstOrDefaultAsync(oi=>oi.ProductId==productId);
        if(hasOrders != null) throw new InvalidOperationException("Cannot delete product with existing orders.Please change the status to 'Discontinued'or'Deleted'");
        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();
    }
    public async Task<string> ChangeProductStatusAsync(Guid userId,Guid productId,ProductStatus newStatus)
    {
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products.FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");

        if(product.Status == ProductStatus.Blocked) throw new InvalidOperationException("Cannot change status of a blocked product.");
        var alowStatus=new List<ProductStatus>
        {
            ProductStatus.Active,
            ProductStatus.Hidden,
            ProductStatus.Discontinued,
            ProductStatus.Deleted,
            ProductStatus.Draft
        };
        if(!alowStatus.Contains(newStatus)) throw new InvalidOperationException("Invalid status transition.");
        if(newStatus == ProductStatus.Active && product.Stock <= 0)
        {
            newStatus = ProductStatus.OutOfStock;
        }
        else
        {
            product.Status = newStatus;
        }
        
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return $"Product status changed to {product.Status}";
        
    }
    ///////////////////////
    /// 
    /// Product Image Management
    /// 
    /// //////////////////////
    
    public async Task<ProductImageDto> AddProductImageAsync(Guid userId,Guid productId,ProductImageCreateDto dto)
    {
        _fileHelper.ValidateImageUrl(dto.ImageUrl);
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var image=new ProductImage
        {
            Id=Guid.NewGuid(),
            ImageUrl=dto.ImageUrl,
            IsMainImage=!product.Images.Any(),
            ProductId=productId
        };
        await _context.ProductImages.AddAsync(image);
        await _productRepository.SaveChangesAsync();
        return new ProductImageDto
        {
            Id=image.Id,
            ImageUrl=image.ImageUrl,
            IsMainImage=image.IsMainImage
        };
        
    }
    public async Task<ProductImageDto> UpdateProductImageAsync(Guid userId,Guid productId,Guid imageId,ProductImageUpdateDto dto)
    {
        _fileHelper.ValidateImageUrl(dto.ImageUrl);
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var image = product.Images.FirstOrDefault(i=>i.Id==imageId);
        if(image == null) throw new KeyNotFoundException("Image not found.");
        image.ImageUrl=dto.ImageUrl;
        if(dto.IsMainImage&& !image.IsMainImage)
        {
            foreach(var img in product.Images)
            {
                img.IsMainImage=false;
            }
            image.IsMainImage=true;
        }

         _context.ProductImages.Update(image);
        await _productRepository.SaveChangesAsync();

        return new ProductImageDto
        {
            Id=image.Id,
            ImageUrl=image.ImageUrl,
            IsMainImage=image.IsMainImage
        };
    }
    public async Task DeleteProductImageAsync(Guid userId,Guid productId,Guid imageId)
    {
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product =await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId== sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var image = product.Images.FirstOrDefault(i=>i.Id==imageId);
        if(image == null) throw new KeyNotFoundException("Image not found.");
        bool wasMainImage=image.IsMainImage;
        _context.ProductImages.Remove(image);
        if (wasMainImage)
        {
            var nextImage =product.Images.FirstOrDefault(i=>i.Id != imageId);
            if(nextImage != null)
            {
                nextImage.IsMainImage=true;
                _context.ProductImages.Update(nextImage);
            }
        }
        await _productRepository.SaveChangesAsync();
    }
    public async Task SetMainProductImageAsync(Guid userId,Guid productId,Guid imageId)
    {
        var sellerid=await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerid == null) throw new KeyNotFoundException("Seller not found.");
        var product=await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==productId&&p.SellerId==sellerid.Id);
        if(product == null) throw new KeyNotFoundException("Product not found.");
        var image=product.Images.FirstOrDefault(i=>i.Id==imageId);
        if(image == null) throw new KeyNotFoundException("Image not found.");
        if(image.IsMainImage) return;
        foreach (var img in product.Images)
        {
            img.IsMainImage=false;
        }
        image.IsMainImage=true;
        _context.ProductImages.Update(image);
        await _productRepository.SaveChangesAsync();
    }
    
}