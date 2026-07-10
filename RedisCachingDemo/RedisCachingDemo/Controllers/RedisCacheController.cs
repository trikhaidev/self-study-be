using Microsoft.AspNetCore.Mvc;
using RedisCachingDemo.Services;

namespace RedisCachingDemo.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class RedisCacheController:ControllerBase
    {
        readonly IRedisCache cache;
        public RedisCacheController(IRedisCache cache)
        {
            this.cache = cache;
        }

        [HttpGet]
        public async Task<List<string>> GetAllKeys()
        {
            return cache.GetAllKeys();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKeysWithData()
        {
            return Ok(await cache.GetAllKeysWithData());
        }
    }
}
