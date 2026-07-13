using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisCachingDemo.Database;
using RedisCachingDemo.Database.Entities;
using RedisCachingDemo.Services;
using System.ComponentModel.DataAnnotations;

namespace RedisCachingDemo.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class LocationController:ControllerBase
    {
        readonly AppDbContext dbContext;
        readonly IRedisCache cache;
        public LocationController(AppDbContext dbContext, IRedisCache cache)
        {
            this.dbContext = dbContext;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            string key = "Countries";
            var data = await cache.TryGetValue<List<Country>>(key);
            if (data == null || !data.Any())
            {
                data = await dbContext.Countries.AsNoTracking().ToListAsync();
                if (data.Any())
                {
                    await cache.Set(key, data, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
            }
            return Ok(data);
        }

        [HttpGet]
        [Route("{Id:required:int:min(1)}")]
        public async Task<IActionResult> GetCountryById([FromRoute][Required]int Id)
        {
            string key = $"Country_{Id}";
            Country? data = await cache.TryGetValue<Country>(key);
            if (data == null)
            {
                data = await dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
                if (data != null)
                {
                    await cache.Set(key,data, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
            }
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            string key = "cities";
            var data = await cache.TryGetValue<List<City>>(key);
            if (data == null || !data.Any())
            {
                data = await dbContext.Cities.AsNoTracking().ToListAsync();
                if (data.Any())
                {
                    await cache.Set(key, data, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
            }
            return Ok(data);
        }

        [HttpGet]
        [Route("{Id:int:required:min(1)}")]
        public async Task<IActionResult> GetCitiesByCountryId([FromRoute][Required] int Id)
        {
            var key = $"country_{Id}_cities";
            var data = await cache.TryGetValue<List<City>>(key);
            if (data == null || !data.Any())
            {
                data = await dbContext.Cities.AsNoTracking().Where(x => x.CountryId == Id).ToListAsync();
                if (data.Any())
                {
                    await cache.Set(key, data, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    });
                }
            }
            return Ok(data);
        }
    }
}
