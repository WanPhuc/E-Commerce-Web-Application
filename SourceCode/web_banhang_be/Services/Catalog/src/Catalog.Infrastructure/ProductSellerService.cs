using Microsoft.Extensions.Configuration;
using VanFucVN.Core.Common.Services;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;

using WebBanHang.Extensions;
using WebBanHang.Helpers.Product;

using AuraMart.Catalog.Dtos;



namespace AuraMart.Catalog.Services;
public class ProductSellerService : IProductSellerService
{
    private readonly IProductRepository _productRepository;
    private readonly ISellerLookup _sellerLookup;
    private readonly ICategoryRepository _categoryRepository;
    private readonly CatalogDbContext _context;
    private readonly FileHelper _fileHelper;
    private readonly ICommonService _commonService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public ProductSellerService(ICommonService commonService, IProductRepository productRepository, ISellerLookup sellerLookup, ICategoryRepository categoryRepository, CatalogDbContext context, FileHelper fileHelper, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _productRepository = productRepository;
        _sellerLookup = sellerLookup;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _categoryRepository = categoryRepository;
        _context = context;
        _fileHelper = fileHelper;
        _commonService = commonService;
    }
    public async Task<ApiResponse<PagedResult<SellerProductDto>>> GetAllProductsAsync(PagedRequest request)
    {
        var userId = _commonService.GetUserId();

        var seller = await _sellerLookup.FindByUserIdAsync(userId);

        if (seller == null)
            return ApiResponse<PagedResult<SellerProductDto>>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var query = _productRepository
            .FindByCondition(p => p.SellerId == seller.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new SellerProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                SKU = p.SKU,
                Price = p.Price,
                Stock = p.Stock,
                LowStockThreshold = p.LowStockThreshold,
                Status = p.Status,
                DiscountPercent = p.DiscountPercent,

                Rating = p.Reviews.Any()
                    ? p.Reviews.Average(r => (double?)r.Rating) ?? 0
                    : 0,

                ReviewCount = p.Reviews.Count,
                SoldCount = p.SoldCount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CreatedAt = p.CreatedAt,

                MainImageUrl = p.Images
                    .OrderByDescending(i => i.IsMainImage)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
            });

        var result = await query.ToPagedAsync(request);

        return ApiResponse<PagedResult<SellerProductDto>>.Success(result, "Success", 200, SuccessCodes.Product.ListRetrieved);
    }
    public async Task<ApiResponse<SellerProductDto>> GetProductByIdAsync(Guid productId)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<SellerProductDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var productid = await _productRepository.FindSingleByConditionAsync(
            p => p.Id == productId && p.SellerId == seller.Id && !p.DeleteFlg,
            false,
            p => p.Category,
            p => p.Reviews,
            p => p.Images);
        if (productid == null) return ApiResponse<SellerProductDto>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);

        var productDtos = new SellerProductDto
        {
            Id = productid.Id,
            Name = productid.Name,
            Description = productid.Description,
            SKU = productid.SKU,
            Price = productid.Price,
            Stock = productid.Stock,
            LowStockThreshold = productid.LowStockThreshold,
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
        return ApiResponse<SellerProductDto>.Success(productDtos, "Success", 200, SuccessCodes.Product.DetailRetrieved);
    }
    public async Task<ApiResponse<SellerProductDto>> CreateProductAsync(CreateSellerProductDto createSellerProductDto)
    {
        var productid = Guid.NewGuid();
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<SellerProductDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var sku = await _productRepository.FindSingleByConditionAsync(p => p.SKU == createSellerProductDto.SKU);
        if (sku != null) return ApiResponse<SellerProductDto>.Fail("SKU already exists.", 400, ErrorCodes.Product.SkuExists);
        var category = await _categoryRepository.GetByIdAsync(createSellerProductDto.CategoryId);
        if (category == null) return ApiResponse<SellerProductDto>.Fail("Category not found.", 404, ErrorCodes.Product.CategoryNotFound);
        var productimage = new List<ProductImage>();
        if (createSellerProductDto.Images != null && createSellerProductDto.Images.Any())
        {
            foreach (var imgDto in createSellerProductDto.Images)
            {
                try
                {
                    _fileHelper.ValidateImageUrl(imgDto.ImageUrl);
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
                {
                    return ApiResponse<SellerProductDto>.Fail(ex.Message, 400, ErrorCodes.Validation.InvalidFormat);
                }

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
        var product = new Product
        {
            Id = productid,
            Name = createSellerProductDto.Name,
            Description = createSellerProductDto.Description,
            SKU = createSellerProductDto.SKU,
            Price = createSellerProductDto.Price,
            Stock = createSellerProductDto.Stock,
            LowStockThreshold = createSellerProductDto.LowStockThreshold,
            Status = createSellerProductDto.Status,
            DiscountPercent = createSellerProductDto.DiscountPercent,
            CategoryId = createSellerProductDto.CategoryId,
            SellerId = seller.Id,
            CreatedAt = DateTime.UtcNow,
            SoldCount = 0,
            Images = productimage
        };

        await _productRepository.CreateAsync(product);
        await _productRepository.SaveChangesAsync();
        var productDto = new SellerProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? "",
            SKU = product.SKU,
            Price = product.Price,
            Stock = product.Stock,
            LowStockThreshold = product.LowStockThreshold,
            Status = product.Status,
            DiscountPercent = product.DiscountPercent,
            Rating = 0,
            ReviewCount = 0,
            SoldCount = product.SoldCount,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            CreatedAt = product.CreatedAt,
            MainImageUrl = product.Images.FirstOrDefault(i => i.IsMainImage)?.ImageUrl ?? product.Images.FirstOrDefault()?.ImageUrl,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsMainImage = i.IsMainImage
            }).ToList()

        };
        return ApiResponse<SellerProductDto>.Success(productDto, "Product created successfully", 201, SuccessCodes.Product.Created);
    }
    public async Task<ApiResponse<SellerProductDto>> UpdateProductAsync(Guid productId, UpdateSellerProductDto dto)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<SellerProductDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<SellerProductDto>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        var exitsSKU = await _productRepository.FindSingleByConditionAsync(p => p.SKU == dto.SKU);
        if (exitsSKU != null && exitsSKU.Id != productId) return ApiResponse<SellerProductDto>.Fail("SKU already exists.", 400, ErrorCodes.Product.SkuExists);
        var categry = await _context.Categories.FindAsync(dto.CategoryId);
        if (categry == null) return ApiResponse<SellerProductDto>.Fail("Category not found.", 404, ErrorCodes.Product.CategoryNotFound);

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.SKU = dto.SKU;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.LowStockThreshold = dto.LowStockThreshold;
        product.Status = dto.Status;
        product.DiscountPercent = dto.DiscountPercent;
        product.CategoryId = dto.CategoryId;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        var productDto = new SellerProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? "",
            SKU = product.SKU,
            Price = product.Price,
            Stock = product.Stock,
            LowStockThreshold = product.LowStockThreshold,
            Status = product.Status,
            DiscountPercent = product.DiscountPercent,
            Rating = product.Reviews.Any() ? product.Reviews.Average(r => (double?)r.Rating) ?? 0 : 0,
            ReviewCount = product.Reviews.Count,
            SoldCount = product.SoldCount,
            CategoryId = product.CategoryId,
            CategoryName = categry.Name,
            CreatedAt = product.CreatedAt,
            MainImageUrl = product.Images.FirstOrDefault(i => i.IsMainImage)?.ImageUrl ?? product.Images.FirstOrDefault()?.ImageUrl,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsMainImage = i.IsMainImage
            }).ToList()

        };
        return ApiResponse<SellerProductDto>.Success(productDto, "Product updated successfully", 200, SuccessCodes.Product.Updated);
    }
    public async Task<ApiResponse<string>> DeleteProductAsync(Guid productId)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<string>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var product = await _productRepository.FindSingleByConditionAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<string>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        // PHASE 6.2: OrderItems thu ve Ordering (core) - kiem tra qua internal API
        var coreClient = _httpClientFactory.CreateClient("core");
        coreClient.DefaultRequestHeaders.Add("X-Internal-Api-Key", _configuration["InternalApi:Key"] ?? "dev-internal-key");
        var hasOrders = await coreClient.GetFromJsonAsync<bool>($"/internal/orders/exists-for-product/{productId}");
        if (hasOrders) return ApiResponse<string>.Fail("Cannot delete product with existing orders.Please change the status to 'Discontinued'or'Deleted'", 400, ErrorCodes.Product.CannotDeleteHasOrders);
        await _productRepository.SoftDeleteAsync(productId);
        await _productRepository.SaveChangesAsync();
        return ApiResponse<string>.Success("Product deleted successfully", "Product deleted successfully", 200, SuccessCodes.Product.Deleted);
    }
    public async Task<ApiResponse<string>> ChangeProductStatusAsync(Guid productId, ProductStatus newStatus)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<string>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<string>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);

        if (product.Status == ProductStatus.Blocked) return ApiResponse<string>.Fail("Cannot change status of a blocked product.", 400, ErrorCodes.Product.Blocked);
        var alowStatus = new List<ProductStatus>
        {
            ProductStatus.Active,
            ProductStatus.Hidden,
            ProductStatus.Discontinued,
            ProductStatus.Deleted,
            ProductStatus.Draft
        };
        if (!alowStatus.Contains(newStatus)) return ApiResponse<string>.Fail("Invalid status transition.", 400, ErrorCodes.Product.InvalidStatusTransition);
        if (newStatus == ProductStatus.Active && product.Stock <= 0)
        {
            product.Status = ProductStatus.OutOfStock;
        }
        else
        {
            product.Status = newStatus;
        }

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return ApiResponse<string>.Success($"Product status changed to {product.Status}", "Product status changed successfully", 200, SuccessCodes.Product.StatusChanged);

    }
    ///////////////////////
    /// 
    /// Product Image Management
    /// 
    /// //////////////////////

    public async Task<ApiResponse<ProductImageDto>> AddProductImageAsync(Guid productId, ProductImageCreateDto dto)
    {
        try
        {
            _fileHelper.ValidateImageUrl(dto.ImageUrl);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return ApiResponse<ProductImageDto>.Fail(ex.Message, 400, ErrorCodes.Validation.InvalidFormat);
        }

        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<ProductImageDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<ProductImageDto>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ImageUrl = dto.ImageUrl,
            IsMainImage = !product.Images.Any(),
            ProductId = productId
        };
        await _context.ProductImages.AddAsync(image);
        await _productRepository.SaveChangesAsync();
        var imageDto = new ProductImageDto
        {
            Id = image.Id,
            ImageUrl = image.ImageUrl,
            IsMainImage = image.IsMainImage
        };
        return ApiResponse<ProductImageDto>.Success(imageDto, "Product image added successfully", 201, SuccessCodes.Product.ImageAdded);

    }
    public async Task<ApiResponse<ProductImageDto>> UpdateProductImageAsync(Guid productId, Guid imageId, ProductImageCreateDto dto)
    {
        try
        {
            _fileHelper.ValidateImageUrl(dto.ImageUrl);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return ApiResponse<ProductImageDto>.Fail(ex.Message, 400, ErrorCodes.Validation.InvalidFormat);
        }

        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<ProductImageDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<ProductImageDto>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null) return ApiResponse<ProductImageDto>.Fail("Image not found.", 404, ErrorCodes.Product.ImageNotFound);
        image.ImageUrl = dto.ImageUrl;
        if (dto.IsMainImage && !image.IsMainImage)
        {
            foreach (var img in product.Images)
            {
                img.IsMainImage = false;
            }
            image.IsMainImage = true;
        }

        _context.ProductImages.Update(image);
        await _productRepository.SaveChangesAsync();

        var imageDto = new ProductImageDto
        {
            Id = image.Id,
            ImageUrl = image.ImageUrl,
            IsMainImage = image.IsMainImage
        };
        return ApiResponse<ProductImageDto>.Success(imageDto, "Product image updated successfully", 200, SuccessCodes.Product.ImageUpdated);
    }
    public async Task<ApiResponse<string>> DeleteProductImageAsync(Guid productId, Guid imageId)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<string>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<string>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null) return ApiResponse<string>.Fail("Image not found.", 404, ErrorCodes.Product.ImageNotFound);
        bool wasMainImage = image.IsMainImage;
        _context.ProductImages.Remove(image);
        if (wasMainImage)
        {
            var nextImage = product.Images.FirstOrDefault(i => i.Id != imageId);
            if (nextImage != null)
            {
                nextImage.IsMainImage = true;
                _context.ProductImages.Update(nextImage);
            }
        }
        await _productRepository.SaveChangesAsync();
        return ApiResponse<string>.Success("Product image deleted successfully", "Product image deleted successfully", 200, SuccessCodes.Product.ImageDeleted);
    }
    public async Task<ApiResponse<bool>> SetMainProductImageAsync(Guid productId, Guid imageId)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<bool>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == seller.Id);
        if (product == null) return ApiResponse<bool>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);
        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null) return ApiResponse<bool>.Fail("Image not found.", 404, ErrorCodes.Product.ImageNotFound);
        if (image.IsMainImage) return ApiResponse<bool>.Success(true, "Main product image set successfully", 200, SuccessCodes.Product.MainImageSet);
        foreach (var img in product.Images)
        {
            img.IsMainImage = false;
        }
        image.IsMainImage = true;
        _context.ProductImages.Update(image);
        await _productRepository.SaveChangesAsync();
        return ApiResponse<bool>.Success(true, "Main product image set successfully", 200, SuccessCodes.Product.MainImageSet);
    }

}
