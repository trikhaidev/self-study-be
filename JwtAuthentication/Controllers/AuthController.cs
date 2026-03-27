using System.ComponentModel.DataAnnotations;
using System.Net;
using JwtAuthentication.Database;
using JwtAuthentication.Models.Request;
using JwtAuthentication.Services;
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
}