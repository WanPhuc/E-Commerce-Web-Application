using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public void SetCache(string key, object value, int? minutes = null)
    {
        var json = JsonSerializer.Serialize(value, JsonOpts);
        var options = new DistributedCacheEntryOptions();
        if (minutes.HasValue)
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes.Value);
        _cache.SetString(key, json, options);
    }

    public T? GetCache<T>(string key)
    {
        var json = _cache.GetString(key);
        if (string.IsNullOrEmpty(json)) return default;
        return JsonSerializer.Deserialize<T>(json, JsonOpts);
    }

    public void ClearCache()
    {
        // Redis: cannot clear all keys safely - no-op
    }

    public void RemoveCache(string key) => _cache.Remove(key);
}
