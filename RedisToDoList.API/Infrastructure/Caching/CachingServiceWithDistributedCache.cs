using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisToDoList.API.Infrastructure.Caching;

public class CachingServiceWithDistributedCache : ICachingService
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _options;

    public CachingServiceWithDistributedCache(IDistributedCache cache)
    {
        _cache = cache;
        _options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(1200)
        };
    }
   
    public async Task<T> GetAsync<T>(string key)
    {
        var cachedData = await _cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        return default;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, _options);
    }

    public async  Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}