using System.ComponentModel.DataAnnotations;
using System.Net;
using JwtAuthentication.Database;
using JwtAuthentication.Models.Request;
using JwtAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Controllers;

[ApiController]
[Route("[Controller]")]
public class AuthController : ControllerBase
{
    readonly ITokenManagementService tokenService;
    public AuthController(ITokenManagementService tokenService)
    {
        this.tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody][Required]LoginRequest model)
    {
        var res = await tokenService.Login(model);
        if (res.IsError)
        {
            return base.StatusCode((int)HttpStatusCode.InternalServerError,res);
        }

        return Ok(res);
    }

    [HttpPost]
    [Route("Refresh")]
    public async Task<IActionResult> RefreshSession([Required][FromForm]string refreshToken)
    {
        var res = await tokenService.RefreshSession(refreshToken);
        if (res.IsError)
        {
            return StatusCode(500,res);
        }
        return Ok(res);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Logout([Required][FromHeader]string Authorization)
    {
        var res = await tokenService.Logout(Authorization);
        if (res.IsError)
        {
            return StatusCode(500,res);
        }
        return Ok(res);
    }
}