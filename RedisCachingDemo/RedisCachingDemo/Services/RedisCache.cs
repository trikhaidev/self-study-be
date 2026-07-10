using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using static RedisCachingDemo.Services.IRedisCache;

namespace RedisCachingDemo.Services
{
    public interface IRedisCache
    {
        public class RedisConfig
        {
            public string Host { get; set; } = null!;
            public string InstanceName { get; set; } = null!;
        }
        Task Set<T>(string key, T value, DistributedCacheEntryOptions options);
        Task Remove(string key);
        Task<T?> TryGetValue<T>(string key);
        List<string> GetAllKeys();
        Task<List<KeyValuePair<string, object>>> GetAllKeysWithData();
    }
    public class RedisCache : IRedisCache
    {
        readonly RedisConfig config;
        readonly IDistributedCache cache;
        readonly IConnectionMultiplexer connection;
        public RedisCache(IOptions<RedisConfig> options, 
            IDistributedCache cache,
            IConnectionMultiplexer connection)
        {
            this.config = options.Value;
            this.cache = cache;
            this.connection = connection;
        }

        public async Task Set<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var json = JsonConvert.SerializeObject(value);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            await cache.SetStringAsync(key, json, options);
        }

        public async Task Remove(string key)
        {
            await cache.RemoveAsync(key);
        }

        public async Task<T?> TryGetValue<T>(string key)
        {
            T? data = default;
            try
            {
                var json = await cache.GetStringAsync(key);
                if (string.IsNullOrWhiteSpace(json))
                {
                    await cache.RemoveAsync(key);
                    return default;
                }
                data = JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
            return data;
        }

        public List<string> GetAllKeys()
        {
            var server = connection.GetServer(connection.GetEndPoints().First());
            var keys = server.Keys().ToList();
            return keys.Select(x => x.ToString().Replace(config.InstanceName,"")).ToList();
        }

        public async Task<List<KeyValuePair<string, object>>> GetAllKeysWithData()
        {
            var server = connection.GetServer(connection.GetEndPoints().First());
            var keys = server.Keys().ToList().Select(x => x.ToString().Replace(config.InstanceName, ""));
            var data = new List<KeyValuePair<string, object>>();
            foreach (var k in keys)
            {
                var json = await cache.GetStringAsync(k);
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }
                var obj = JsonConvert.DeserializeObject(json);
                data.Add(new KeyValuePair<string, object>(k, json));
            }
            return data;
        }
    }
}
