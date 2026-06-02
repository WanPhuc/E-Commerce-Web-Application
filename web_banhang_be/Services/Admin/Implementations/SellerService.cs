using Microsoft.AspNetCore.Http.HttpResults;
using WebBanHang.Extensions;
using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs;
using WebBanHang.Models.DTOs.Admin.Sellers;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Interfaces;
namespace WebBanHang.Services.Implementations;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepo;
    private readonly ISellerApplicationRepository _appRepo;
    public SellerService(ISellerRepository sellerRepo, ISellerApplicationRepository appRepo)
    {
        _sellerRepo = sellerRepo;
        _appRepo = appRepo;
    }
    public async Task<ApiResponse<SellerManagementVM>> GetAllSellersAsync(PagedRequest request)
    {
        var pendingAppsQuery = _appRepo
            .FindByCondition(
                sa => sa.Status == SellerApplicationStatus.Pending && !sa.DeleteFlg,
                false,
                sa => sa.User);

        var approvedSellersQuery = _sellerRepo
            .FindByCondition(
                s => !s.DeleteFlg,
                false,
                s => s.User,
                s => s.Address);

        var pendingCount = await _appRepo.CountByConditionAsync(sa => sa.Status == SellerApplicationStatus.Pending && !sa.DeleteFlg);

        var sellerApplications = await pendingAppsQuery
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new SellerApplicationDto
            {
                Id = a.Id,
                StoreName = a.ShopName,
                Email = a.User.Email,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
            })
            .ToPagedAsync(request);

        var approvedSellers = await approvedSellersQuery
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SellerDto
            {
                Id = s.Id,
                StoreName = s.StoreName,
                Email = s.User.Email,
                CreatedAt = s.CreatedAt,
                Status = s.Status,
                Address = new SellerAdressDto
                {
                    PhoneNumber = s.Address.PhoneNumber,
                    District = s.Address.District,
                    City = s.Address.City
                }
            })
            .ToPagedAsync(request);

        var result = new SellerManagementVM
        {
            PendingSellerApplications = pendingCount,
            SellerApplications = sellerApplications,
            ApprovedSellers = approvedSellers

        };

        return ApiResponse<SellerManagementVM>.Success(result, "Success", 200, SuccessCodes.Seller.ManagementRetrieved);
    }
    public async Task<ApiResponse<SellerDetailDto>> GetSellerDetailByIdAsync(Guid sellerId)
    {
        var detailseller = await _sellerRepo.GetByIdAsync(sellerId, s => s.User, s => s.Products);
        if (detailseller == null) return ApiResponse<SellerDetailDto>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var seller = new SellerDetailDto
        {
            Id = detailseller.Id,
            StoreName = detailseller.StoreName,
            Description = detailseller.Description,
            CreatedAt = detailseller.CreatedAt,
            Status = detailseller.Status,

            UserId = detailseller.UserId,
            FullName = detailseller.User.FullName,
            Email = detailseller.User.Email,
            ProductCount = detailseller.Products.Count()
        };
        return ApiResponse<SellerDetailDto>.Success(seller, "Success", 200, SuccessCodes.Seller.DetailRetrieved);


    }

}
