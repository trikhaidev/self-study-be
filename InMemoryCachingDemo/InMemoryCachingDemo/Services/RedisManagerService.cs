using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace InMemoryCachingDemo.Services;

public interface IRedisManagerService
{
    Task<T?> Set<T>(string key, T value, DistributedCacheEntryOptions options);
    bool TryGetValue<T>(string key, out T? data);
    Task Remove(string key);
}
public class RedisManagerService : IRedisManagerService
{
    readonly IDistributedCache cache;
    public RedisManagerService(IDistributedCache cache)
    {
        this.cache = cache;
    }

    public async Task<T?> Set<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        var jsonString = JsonConvert.SerializeObject(value);
        if (!string.IsNullOrWhiteSpace(jsonString))
        {
            await cache.SetStringAsync(key, jsonString, options);
        }
        return value;
    }

    public async Task Remove(string key)
    {
        await cache.RemoveAsync(key);
    }

    public bool TryGetValue<T>(string key, out T? data)
    {
        try
        {
            var json = cache.GetStringAsync(key).Result;
            if (string.IsNullOrWhiteSpace(json))
            {
                data = default;
                return false;
            }

            data = JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            data = default;
            return false;
        }
        return true;
    }
}