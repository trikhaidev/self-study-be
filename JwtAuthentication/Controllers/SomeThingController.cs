using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthentication.Controllers;

[ApiController]
[Route("[Controller]")]
[Authorize]
public class SomeThingController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<string>> GetSomeThing()
    {
        return Ok("Hello World!");
    }
}