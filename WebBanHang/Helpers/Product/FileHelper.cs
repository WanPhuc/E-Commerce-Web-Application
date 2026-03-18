namespace WebBanHang.Helpers.Product;
public class FileHelper
{
    private readonly string[] _allowedExtensions ={ ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public void ValidateImageUrl(string url)
    {
        if(String.IsNullOrEmpty(url)) throw new ArgumentException("Image URL cannot be null or empty.");

        if(!Uri.TryCreate(url,UriKind.Absolute,out _)) throw new ArgumentException("Invalid URL format.");

        var hasValidExtension=_allowedExtensions.Any(ext=>url.Split('?')[0].EndsWith(ext,StringComparison.OrdinalIgnoreCase));
        if(!hasValidExtension) throw new ArgumentException("Invalid image file type. Allowed types are: " + string.Join(", ", _allowedExtensions));
    }
    public void ValidateImageFile(IFormFile file)
    {
        if(file == null||file.Length == 0) throw new ArgumentException("Image file cannot be empty.");
        if(file.Length > _maxFileSize) throw new ArgumentException($"Image file size cannot exceed {_maxFileSize / (1024 * 1024)} MB.");

        var extension =Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Đuôi file không hợp lệ.");

        if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            throw new InvalidOperationException("Loại file không phải là ảnh hợp lệ.");
    }
}