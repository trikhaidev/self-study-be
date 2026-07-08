using InMemoryCachingDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCachingDemo.Controllers;

[ApiController]
[Route("[Controller]/[Action]")]
public class CacheManagerController : ControllerBase
{
    readonly ICacheManagerService cache;
    public CacheManagerController(ICacheManagerService cache)
    {
        this.cache = cache;
        Console.WriteLine("Cache Manager: " + cache.GetHashCode());
    }

    [HttpGet]
    public IActionResult GetAllKeys()
    {
        return Ok(cache.GetAllKeys());
    }

    [HttpDelete]
    public IActionResult ClearAll()
    {
        cache.ClearAll();
        return Ok();
    }

    [HttpDelete]
    public IActionResult Remove()
    {
        
    }
}