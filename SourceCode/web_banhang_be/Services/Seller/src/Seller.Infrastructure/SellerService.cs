namespace AuraMart.Seller.Infrastructure;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepo;
    private readonly ISellerApplicationRepository _appRepo;
    private readonly SellerDbContext _db;

    public SellerService(ISellerRepository sellerRepo, ISellerApplicationRepository appRepo, SellerDbContext db)
    {
        _sellerRepo = sellerRepo;
        _appRepo = appRepo;
        _db = db;
    }

    public async Task<ApiResponse<SellerApplicationResponseDto>> ApplyAsync(Guid userId, RegisterSellerRequest request)
    {
        var existing = await _appRepo.GetByUserIdAsync(userId);
        if (existing != null && existing.Status == "Pending")
        {
            return ApiResponse<SellerApplicationResponseDto>.Fail("Bạn đã có đơn đăng ký đang chờ duyệt", 400);
        }

        var app = new SellerApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShopName = request.ShopName,
            Description = request.Description,
            PhoneNumber = request.PhoneNumber,
            City = request.City,
            District = request.District,
            Ward = request.Ward,
            AddressLine = request.AddressLine,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _db.SellerApplications.Add(app);
        await _db.SaveChangesAsync();

        return ApiResponse<SellerApplicationResponseDto>.Success(MapToAppDto(app), "Gửi yêu cầu đăng ký Seller thành công", 201);
    }

    public async Task<ApiResponse<SellerResponseDto?>> GetMySellerProfileAsync(Guid userId)
    {
        var seller = await _sellerRepo.GetByUserIdAsync(userId);
        if (seller == null)
        {
            return ApiResponse<SellerResponseDto?>.Fail("Tài khoản chưa phải là Seller", 404);
        }
        return ApiResponse<SellerResponseDto?>.Success(MapToSellerDto(seller));
    }

    public async Task<ApiResponse<SellerResponseDto?>> GetSellerByIdAsync(Guid id)
    {
        var seller = await _sellerRepo.GetByIdAsync(id);
        if (seller == null)
        {
            return ApiResponse<SellerResponseDto?>.Fail("Không tìm thấy người bán", 404);
        }
        return ApiResponse<SellerResponseDto?>.Success(MapToSellerDto(seller));
    }

    public async Task<ApiResponse<List<SellerApplicationResponseDto>>> GetPendingApplicationsAsync()
    {
        var list = await _appRepo.GetPendingApplicationsAsync();
        var dtos = list.Select(MapToAppDto).ToList();
        return ApiResponse<List<SellerApplicationResponseDto>>.Success(dtos);
    }

    public async Task<ApiResponse<bool>> ApproveApplicationAsync(Guid applicationId)
    {
        var app = await _appRepo.GetByIdAsync(applicationId);
        if (app == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy đơn đăng ký", 404);
        }

        if (app.Status != "Pending")
        {
            return ApiResponse<bool>.Fail($"Đơn đăng ký đã ở trạng thái {app.Status}", 400);
        }

        app.Status = "Approved";
        app.ReviewedAt = DateTime.UtcNow;
        app.UpdatedAt = DateTime.UtcNow;

        var seller = new Domain.Seller
        {
            Id = Guid.NewGuid(),
            UserId = app.UserId,
            StoreName = app.ShopName,
            Description = app.Description,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow
        };

        _db.Sellers.Add(seller);
        await _db.SaveChangesAsync();

        return ApiResponse<bool>.Success(true, "Duyệt đơn đăng ký Seller thành công");
    }

    private static SellerApplicationResponseDto MapToAppDto(SellerApplication a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        ShopName = a.ShopName,
        Description = a.Description,
        PhoneNumber = a.PhoneNumber,
        City = a.City,
        District = a.District,
        Ward = a.Ward,
        AddressLine = a.AddressLine,
        Status = a.Status,
        CreatedAt = a.CreatedAt
    };

    private static SellerResponseDto MapToSellerDto(Domain.Seller s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        StoreName = s.StoreName,
        Description = s.Description,
        Status = s.Status,
        AddressId = s.AddressId,
        CreatedAt = s.CreatedAt
    };
}
