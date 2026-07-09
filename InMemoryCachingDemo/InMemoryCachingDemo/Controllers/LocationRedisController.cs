using InMemoryCachingDemo.Database;
using InMemoryCachingDemo.Database.Entites;
using InMemoryCachingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace InMemoryCachingDemo.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class LocationRedisController : ControllerBase
    {
        readonly IRedisManagerService cache;
        readonly AppDbContext dbContext;
        public LocationRedisController(IRedisManagerService cache, AppDbContext dbContext)
        {
            this.cache = cache;
            this.dbContext = dbContext;
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
    }
}
