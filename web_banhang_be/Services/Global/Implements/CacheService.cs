using Microsoft.Extensions.Caching.Memory;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements
{
    public class CacheService:ICacheService
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public void SetCache(string key, object value, int? minutes = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal
            };
            if(minutes.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes.Value);
            }
            _cache.Set(key, value, options);
        }
        public T? GetCache<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }
            return default;
        }
        public  void ClearCache()
        {
            _cache.Dispose();
        }
        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
