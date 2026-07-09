using InMemoryCachingDemo.Database;
using InMemoryCachingDemo.Database.Entites;
using InMemoryCachingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace InMemoryCachingDemo.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class LocationRedisController : ControllerBase
    {
        readonly IRedisManagerService cache;
        readonly AppDbContext dbContext;
        readonly IConnectionMultiplexer connection;
        public LocationRedisController(
            IRedisManagerService cache,
            IConnectionMultiplexer connection, 
            AppDbContext dbContext)
        {
            this.cache = cache;
            this.dbContext = dbContext;
            this.connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            string cacheKey = "Countries";
            if (!cache.TryGetValue(cacheKey, out List<Country>? data))
            {
                data = await dbContext.Countries.AsNoTracking().ToListAsync();
                if (data.Any())
                {
                    await cache.Set(cacheKey, data, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(15),
                    });
                }
            }
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetAllKeys()
        {
            var endpoint = connection.GetEndPoints().First();
            var server = connection.GetServer(endpoint);
            var keys = server.Keys().ToList();
            return Ok(keys.Select(x => x.ToString().Replace("Test","")));
        }
    }
}
