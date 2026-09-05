using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebBanHang.Services.Interfaces;
using System.Net.Http.Headers;
using WebBanHang.Helpers.Product;

namespace WebBanHang.Services.Implementations;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly FileHelper _fileHelper;

    public SupabaseStorageService(HttpClient httpClient, IConfiguration configuration, FileHelper fileHelper)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _fileHelper = fileHelper;
    }

    public async Task<string> UploadProductImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        _fileHelper.ValidateImageFile(file);

        var supabaseUrl = _configuration["Supabase:Url"]?.TrimEnd('/');
        var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];
        var bucket = _configuration["Supabase:StorageBucket"] ?? "product-images";

        if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(serviceRoleKey))
        {
            throw new InvalidOperationException("Supabase storage is not configured.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var objectPath = $"products/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";
        var uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucket}/{objectPath}";

        await using var stream = file.OpenReadStream();
        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
        request.Headers.Add("apikey", serviceRoleKey);
        request.Headers.Add("x-upsert", "false");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Supabase upload failed: {(int)response.StatusCode} {body}");
        }

        return $"{supabaseUrl}/storage/v1/object/public/{bucket}/{objectPath}";
    }
}
