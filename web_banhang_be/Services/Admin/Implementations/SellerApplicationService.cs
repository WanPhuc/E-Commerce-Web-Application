using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.EntityModels;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Interfaces;
namespace WebBanHang.Services.Implementations;

public class SellerApplicationService : ISellerApplicationService
{
    private readonly ISellerApplicationRepository _appRepo;
    private readonly ISellerRepository _sellerRepo;
    public SellerApplicationService(ISellerApplicationRepository appRepo, ISellerRepository sellerRepo)
    {
        _appRepo = appRepo;
        _sellerRepo = sellerRepo;
    }

    public async Task<ApiResponse<SellerApplicationDetailDto>> GetSellerApplicationDetailByIdAsync(Guid applicationId)
    {
        var detailApp = await _appRepo.GetByIdAsync(applicationId, a => a.User);
        if (detailApp == null) return ApiResponse<SellerApplicationDetailDto>.Fail("Seller application not found.", 404, ErrorCodes.Seller.ApplicationNotFound);
        var app = new SellerApplicationDetailDto
        {
            Id = detailApp.Id,
            StoreName = detailApp.ShopName,
            Description = detailApp.Description,
            CreatedAt = detailApp.CreatedAt,
            Status = detailApp.Status,

            UserId = detailApp.UserId,
            FullName = detailApp.User.FullName,
            Email = detailApp.User.Email,
            ReviewedAt = detailApp.ReviewedAt
        };
        return ApiResponse<SellerApplicationDetailDto>.Success(app, "Success", 200, SuccessCodes.Seller.ApplicationRetrieved);

    }

    public async Task<ApiResponse<object?>> ApproveSellerApplicationAsync(Guid applicaionId)
    {
        var app = await _appRepo.GetByIdAsync(applicaionId);
        if (app == null) { return ApiResponse<object?>.Fail("Seller application not found.", 404, ErrorCodes.Seller.ApplicationNotFound); }

        if (app.Status != SellerApplicationStatus.Pending) { return ApiResponse<object?>.Fail("Only pending applications can be approved.", 400, ErrorCodes.Validation.InvalidValue); }

        var sellerExits = await _sellerRepo.FindSingleByConditionAsync(s => s.UserId == app.UserId && !s.DeleteFlg);
        if (sellerExits != null)
        {
            return ApiResponse<object?>.Fail("The user is already a seller.", 400, ErrorCodes.Seller.AlreadySeller);
        }

        app.Status = SellerApplicationStatus.Approved;
        app.ReviewedAt = DateTime.UtcNow;
        await _appRepo.UpdateAsync(app);

        var seller = new Models.EntityModels.Seller
        {
            UserId = app.UserId,
            StoreName = app.ShopName,
            Description = app.Description,
            CreatedAt = DateTime.UtcNow,
            Status = SellerApplicationStatus.Approved,
            Address = new Address
            {
                RecipientName = app.ShopName,
                PhoneNumber = app.PhoneNumber,
                City = app.City,
                District = app.District,
                AddressLine = app.AddressLine,
                Ward = app.Ward,
                IsDefault = true,
                UserId = app.UserId
            }
        };


        await _sellerRepo.CreateAsync(seller);
        return ApiResponse<object?>.Success(null, "Seller application approved successfully.", 200, SuccessCodes.Seller.ApplicationApproved);

    }
    public async Task<ApiResponse<object?>> RejectSellerApplicationAsync(Guid applicationId)
    {
        var app = await _appRepo.GetByIdAsync(applicationId);
        if (app == null) { return ApiResponse<object?>.Fail("Seller application not found.", 404, ErrorCodes.Seller.ApplicationNotFound); }
        if (app.Status != SellerApplicationStatus.Pending) return ApiResponse<object?>.Fail("Only pending applications can be rejected.", 400, ErrorCodes.Validation.InvalidValue);

        var sellerExits = await _sellerRepo.FindSingleByConditionAsync(s => s.UserId == app.UserId && !s.DeleteFlg);
        if (sellerExits != null)
        {
            return ApiResponse<object?>.Fail("The user is already a seller.", 400, ErrorCodes.Seller.AlreadySeller);
        }

        app.Status = SellerApplicationStatus.Rejected;
        app.ReviewedAt = DateTime.UtcNow;
        await _appRepo.UpdateAsync(app);
        return ApiResponse<object?>.Success(null, "Seller application rejected successfully.", 200, SuccessCodes.Seller.ApplicationRejected);

    }

}
