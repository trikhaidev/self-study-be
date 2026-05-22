using Microsoft.AspNetCore.Mvc;

namespace WebSomeThing.Controllers;
[ApiController]
[Route("[Controller]")]
public class SomeThingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                Console.WriteLine("Background Task is running...");
                await Task.Delay(1000);
            }
        });
        return Ok("Hello world!");
    }
}