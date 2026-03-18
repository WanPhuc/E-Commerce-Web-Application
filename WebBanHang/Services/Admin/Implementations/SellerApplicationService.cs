using WebBanHang.Models;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories;
using WebBanHang.Services.Interfaces;
namespace WebBanHang.Services.Implementations;
public class SellerApplicationService : ISellerApplicationService{
    private readonly ISellerApplicationRepository _appRepo;
    private readonly ISellerRepository _sellerRepo;
    public SellerApplicationService(ISellerApplicationRepository appRepo, ISellerRepository sellerRepo){
        _appRepo = appRepo;
        _sellerRepo = sellerRepo;
    }

    public async Task<SellerApplicationDetailDto> GetSellerApplicationDetailByIdAsync(Guid applicationId)
    {
        var detailApp = await _appRepo.GetByIdAsync(applicationId);
        if (detailApp == null) throw new KeyNotFoundException("Seller application not found.");
        var app = new SellerApplicationDetailDto
        {
            Id=detailApp.Id,
            StoreName=detailApp.ShopName,
            Description=detailApp.Description,
            CreatedAt=detailApp.CreatedAt,
            Status=detailApp.Status,

            UserId=detailApp.UserId,
            FullName=detailApp.User.FullName,
            Email=detailApp.User.Email,
            ReviewedAt=detailApp.ReviewedAt
        };
        return app;

    }
    
    public async Task ApproveSellerApplicationAsync(Guid applicaionId)
    {
        var app = await _appRepo.GetByIdAsync(applicaionId);
        if(app == null){throw new KeyNotFoundException("Seller application not found.");}

        if(app.Status != SellerApplicationStatus.Pending){throw new InvalidOperationException("Only pending applications can be approved.");}

        var sellerExits=await _sellerRepo.GetSellerByUserIdAsync(app.UserId);
        if(sellerExits != null)
        {
            throw new InvalidOperationException("The user is already a seller.");
        }

        app.Status = SellerApplicationStatus.Approved;
        app.ReviewedAt=DateTime.UtcNow;
        await _appRepo.UpdateAsync(app);

        var seller = new Models.Seller
        {
            UserId=app.UserId,
            StoreName=app.ShopName,
            Description=app.Description,
            CreatedAt=DateTime.UtcNow,
            Status=SellerApplicationStatus.Approved,
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

        
        await _sellerRepo.AddAsync(seller);

        await _sellerRepo.SaveChangesAsync();
    }
    public async Task RejectSellerApplicationAsync(Guid applicationId)
    {
        var app=await _appRepo.GetByIdAsync(applicationId);
        if(app == null){throw new KeyNotFoundException("Seller application not found.");}
        if(app.Status != SellerApplicationStatus.Pending) throw new InvalidOperationException("Only pending applications can be rejected.");

        var sellerExits=await _sellerRepo.GetSellerByUserIdAsync(app.UserId);
        if(sellerExits != null)
        {
            throw new InvalidOperationException("The user is already a seller.");
        }

        app.Status = SellerApplicationStatus.Rejected;
        app.ReviewedAt=DateTime.UtcNow;
        await _appRepo.UpdateAsync(app);
        await _appRepo.SaveChangesAsync();
        
    }
    
}