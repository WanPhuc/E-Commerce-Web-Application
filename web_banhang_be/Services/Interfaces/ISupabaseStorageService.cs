namespace WebBanHang.Services.Interfaces;

public interface ISupabaseStorageService
{
    Task<string> UploadProductImageAsync(IFormFile file, CancellationToken cancellationToken = default);
}

