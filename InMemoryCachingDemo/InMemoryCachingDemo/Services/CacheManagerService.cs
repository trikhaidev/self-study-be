using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCachingDemo.Services;

public interface ICacheManagerService
{
    T? Set<T>(string key, T value, MemoryCacheEntryOptions options);

    bool TryGetValue<T>(string key, out T? value);

    void Remove(string key);

    List<string> GetAllKeys();

    void ClearAll();
}
public class CacheManagerService : ICacheManagerService
{
    readonly IMemoryCache cache;
    readonly ConcurrentDictionary<string, bool> cacheKeys = new ConcurrentDictionary<string, bool>();
    public CacheManagerService(IMemoryCache cache)
    {
        this.cache = cache;
        Console.WriteLine("Memory cache: "+cache.GetHashCode());
    }

    public T? Set<T>(string key, T value, MemoryCacheEntryOptions options)
    {
        cache.Set(key, value, options);
        cacheKeys.TryAdd(key, true);
        return value;
    }

    public void Remove(string key)
    {
        cache.Remove(key);
        cacheKeys.TryRemove(key, out _);
    }

    public void ClearAll()
    {
        foreach (var key in cacheKeys.Keys)
        {
            cache.Remove(key);
        }
        cacheKeys.Clear();
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (cache.TryGetValue(key, out value))
        {
            return true;
        }
        cacheKeys.TryRemove(key, out _);
        return false;
    }

    public List<string> GetAllKeys()
    {
        return cacheKeys.Select(x => x.Key).ToList();
    }
}