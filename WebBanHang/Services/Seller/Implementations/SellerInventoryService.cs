using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.DTOs.Seller.Inventory;
using WebBanHang.Repositories;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Services.Seller.Implementations;

public class SellerInventoryService : ISellerInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly ISellerRepository _sellerRepository;
    public SellerInventoryService(IProductRepository productRepository, ISellerRepository sellerRepository)
    {
        _productRepository = productRepository;
        _sellerRepository = sellerRepository;
    }
    public async Task<List<SellerInventoryDto>> GetSellerInventoryAsync(Guid userId, string? filter = null)
    {
        var seller = await _sellerRepository.GetSellerByUserIdAsync(userId);
        if (seller == null) throw new KeyNotFoundException("Seller not found.");
        var productsQuery = _productRepository.GetQueryableProductsBySeller(seller.Id);
        productsQuery = filter?.ToLower() switch
        {
            "lowstock" => productsQuery.Where(p => p.Stock <= p.LowStockThreshold && p.Stock > 0),
            "outofstock" => productsQuery.Where(p => p.Stock == 0),
            _ => productsQuery
        };

        var productDtos = await productsQuery.Select(p => new SellerInventoryDto
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.SKU,
            Stock = p.Stock,
            LowStockThreshold = p.LowStockThreshold,
            Status = p.Status,
            ImageUrl = p.Images.OrderByDescending(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault(),
            Price = p.Price,
        }).ToListAsync();
        return productDtos;
    }
    public async Task UpdateSellerInventoryAsync(Guid productId, UpdateSellerInventoryDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) throw new KeyNotFoundException("Product not found.");

        product.Stock = updateDto.Stock ?? product.Stock;
        product.LowStockThreshold = updateDto.LowStockThreshold ?? product.LowStockThreshold;
        product.Status = updateDto.Status ?? product.Status;

        await _productRepository.UpdateAsync(product);
    }
}