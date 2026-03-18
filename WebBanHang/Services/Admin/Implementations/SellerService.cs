using Microsoft.AspNetCore.Http.HttpResults;
using WebBanHang.Models;
using WebBanHang.Models.DTOs;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;
using WebBanHang.Models.ViewModels;
using WebBanHang.Repositories;
using WebBanHang.Services.Interfaces;
namespace WebBanHang.Services.Implementations;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepo;
    private readonly ISellerApplicationRepository _appRepo;
    public SellerService(ISellerRepository sellerRepo, ISellerApplicationRepository appRepo){
        _sellerRepo = sellerRepo;
        _appRepo = appRepo;
    }
    public async Task<SellerManagementVM> GetAllSellersAsync()
    {
       var pendingApps = await _appRepo.GetSellerApplicationsByStatusPendingAsync();
       var approvedSellers = await _sellerRepo.GetAllAsync();

       var result = new SellerManagementVM
       {
           PendingSellerApplications = pendingApps.Count,SellerApplications=pendingApps.Select(a=> new SellerApplicationDto
           {
               Id=a.Id,
               ShopName=a.ShopName,
               Email=a.User.Email,
               Status=a.Status,
               CreatedAt=a.CreatedAt,

           }).ToList(),
           ApprovedSellers=approvedSellers.Select(s=>new SellerDto
           {
               Id=s.Id,
               StoreName=s.StoreName,
               Email=s.User.Email,
                CreatedAt=s.CreatedAt,
                Status=SellerApplicationStatus.Approved,
                Address=new SellerAdressDto
                {
                    PhoneNumber = s.Address.PhoneNumber,
                    District=s.Address.District,
                    City=s.Address.City
                }
                
           }).ToList()

       };

       return result;
    }
    public async Task<SellerDetailDto> GetSellerDetailByIdAsync(Guid sellerId)
    {
        var detailseller =await _sellerRepo.GetByIdAsync(sellerId);
        if(detailseller==null) throw new  KeyNotFoundException("Seller not found."); 
        var seller=new SellerDetailDto
        {
            Id=detailseller.Id,
            StoreName=detailseller.StoreName,
            Description=detailseller.Description,
            CreatedAt=detailseller.CreatedAt,
            Status=detailseller.Status,

            UserId=detailseller.UserId,
            FullName=detailseller.User.FullName,
            Email=detailseller.User.Email,
            ProductCount=detailseller.Products.Count()
        };
        return seller;
            
        
    }

}