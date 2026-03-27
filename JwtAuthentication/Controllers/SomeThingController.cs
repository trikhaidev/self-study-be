using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthentication.Controllers;

[ApiController]
[Route("[Controller]")]
[Authorize]
public class SomeThingController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> GetSomeThing()
    {
        return Ok("Hello World!");
    }

    [HttpGet]
    [Route("Roles")]
    // [Authorize(Roles = "Guest")]
    [Authorize(Roles = "Staff,User,Admin")]
    public async Task<ActionResult<string>> GetSomeThingByRoles()
    {
        var roles = this.HttpContext.User.FindAll(ClaimTypes.Role).Select(x => x.Value);
        var iden = this.HttpContext.User.Identities.Select(x => x.NameClaimType);
        return Ok(new
        {
            roles = roles,
            auths = iden
        });
    }
}