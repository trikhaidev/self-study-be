using Microsoft.AspNetCore.Mvc;

namespace WebSomeThing.Controllers;
[ApiController]
[Route("[Controller]")]
public class SomeThingController : ControllerBase
{
    [HttpGet]
    public async IActionResult Get()
    {
        // Task.Run(async () =>
        // {
        //     while (true)
        //     {
        //         Console.WriteLine("Background Task is running...");
        //         await Task.Delay(1000);
        //     }
        // });

        // var thread = new Thread(() =>
        // {
        //     while (true)
        //     {
        //         Console.WriteLine("Background Thread is running...");
        //         Thread.Sleep(1000);
        //     }
        // });
        // thread.IsBackground = true;
        //thread.Start();

        var task = new Task<string>(() =>
        {
            return "Hello";
        });
        task.Start();
        task.Wait();
        task.Result.ToString();
        return Ok("Hello world!");
    }
}