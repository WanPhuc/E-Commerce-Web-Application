namespace BuildingBlocks.Redis;

public interface ICacheService
{
    void SetCache(string key, object value, int? minutes = null);
    T? GetCache<T>(string key);
    void ClearCache();
    void RemoveCache(string key);
}
