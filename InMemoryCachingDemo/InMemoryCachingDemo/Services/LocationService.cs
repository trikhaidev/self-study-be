using InMemoryCachingDemo.Database;
using InMemoryCachingDemo.Database.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCachingDemo.Services
{
    public interface ILocationService
    {
        Task<Country> AddCountry(string Name, string NameOfVietnamese);
        Task<bool> UpdateCountry(int Id, string Name, string NameOfVietnamese);
        Task<List<Country>> GetCountries();
        Task<Country?> GetCountryById(int Id);
        Task<List<City>> GetCities();
    }
    public class LocationService : ILocationService
    {
        readonly AppDbContext dbContext;
        readonly ICacheManagerService cache;
        public LocationService(AppDbContext dbContext,
                                ICacheManagerService cache)
        {
            this.dbContext = dbContext;
            this.cache = cache;
        }

        public async Task<Country> AddCountry(string Name, string NameOfVietnamese)
        {
            var data = new Country
            {
                Name = Name,
                NameOfVietNamese = NameOfVietnamese
            };
            await dbContext.AddAsync(data);
            await dbContext.SaveChangesAsync();
            cache.Remove("countries");
            return data;
        }

        public async Task<bool> UpdateCountry(int Id, string Name, string NameOfVietnamese)
        {
            var data = await dbContext.Countries.FirstOrDefaultAsync(x => x.Id == Id);
            if (data == null)
            {
                return false;
            }
            data.Name = Name.Trim();
            data.NameOfVietNamese = NameOfVietnamese.Trim();
            await dbContext.SaveChangesAsync();
            cache.Remove("countries");
            cache.Set($"country_{Id}",data, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.High
            });
            return true;
        }

        public async Task<List<Country>> GetCountries()
        {
            string cacheKey = "countries";
            if (!cache.TryGetValue(cacheKey, out List<Country>? data))
            {
                data = await dbContext.Countries.AsNoTracking().ToListAsync();
                if (data.Any())
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                                        .SetPriority(CacheItemPriority.High);
                    cache.Set(cacheKey,data,cacheOptions);
                }
            }
            return data ?? new List<Country>();
        }

        public async Task<Country?> GetCountryById(int Id)
        {
            string cacheKey = $"country_{Id}";
            if (!cache.TryGetValue(cacheKey,out Country? data))
            {
                data = await dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
                if (data != null)
                {
                    cache.Set(cacheKey,data, new MemoryCacheEntryOptions
                    {
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(15)
                    });
                }
            }
            return data;
        }

        public async Task<List<City>> GetCities()
        {
            string cacheKey = "cities";
            if (!cache.TryGetValue(cacheKey, out List<City>? data))
            {
                data = await dbContext.Cities.AsNoTracking().ToListAsync();
                if (data.Any())
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                                        .SetPriority(CacheItemPriority.Low);
                    cache.Set(cacheKey,data,cacheOptions);
                }
            }
            return data ?? new List<City>();
        }
    }
}
