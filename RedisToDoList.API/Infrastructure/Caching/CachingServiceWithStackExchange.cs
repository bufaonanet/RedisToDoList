using System.Text.Json;
using StackExchange.Redis;

namespace RedisToDoList.API.Infrastructure.Caching;

public class CachingServiceWithStackExchange : ICachingService
{
    private IDatabase _database;

    public CachingServiceWithStackExchange()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _database = redis.GetDatabase();
    }
    
    public async Task<T> GetAsync<T>(string key)
    {
        string cachedData = await _database.StringGetAsync(key);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        return default;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        var expiration = TimeSpan.FromHours(1);
        await _database.StringSetAsync(key, serializedValue, expiration);
    }

   

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}