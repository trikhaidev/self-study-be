using Microsoft.Extensions.Caching.Distributed;

namespace InMemoryCachingDemo.Services;

public interface IRedisManagerService
{
    T? Set<T>(string key, T value, DistributedCacheEntryOptions options);
}
public class RedisManagerService : IRedisManagerService
{
    readonly IDistributedCache cache;
    public RedisManagerService(IDistributedCache cache)
    {
        this.cache = cache;
    }

    public T? Set<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        
    }
}