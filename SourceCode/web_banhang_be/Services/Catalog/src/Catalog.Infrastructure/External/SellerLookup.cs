using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using AuraMart.Catalog.Application;

namespace AuraMart.Catalog.Infrastructure.External;

public class HttpSellerLookup(IHttpClientFactory factory, IConfiguration configuration) : ISellerLookup
{
    public async Task<SellerRef?> FindByUserIdAsync(Guid userId)
    {
        var client = factory.CreateClient("core");
        client.DefaultRequestHeaders.Add("X-Internal-Api-Key", configuration["InternalApi:Key"] ?? "dev-internal-key");
        return await client.GetFromJsonAsync<SellerRef>($"/internal/sellers/by-user/{userId}");
    }
}
