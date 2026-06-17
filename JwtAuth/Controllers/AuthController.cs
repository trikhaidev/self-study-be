using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers;
[ApiController]
[Route("[Controller]")]
public class AuthController : ControllerBase
{
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }
    readonly IAuthService authService;

    [HttpPost]
    public async Task<IActionResult> Login([FromBody]AuthRequestModel_Login model)
    {
        try
        {
            var res = await authService.Login(model.UserName,model.Password, HttpContext.Response);
            return StatusCode(res.StatusCode,res);
        }
        catch(Exception e)
        {
            return StatusCode(500, new ResponseBaseModel<object>
            {
                IsOk = false,
                StatusCode = 500,
                Message = e.Message
            });
        }
    }

    [HttpPut]
    [Route("/[Controller]")]
    [Route("[Action]")]
    public async Task<IActionResult> RefreshSession()
    {
        try
        {
            if (!HttpContext.Request.Cookies.TryGetValue("refresh_token", out string? refreshToken)
                || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            };

            var res = await authService.RefreshSession(refreshToken, HttpContext.Response);
            return StatusCode(res.StatusCode,res);
        }
        catch(Exception e)
        {
            return StatusCode(500, new ResponseBaseModel<object>
            {
                IsOk = false,
                StatusCode = 500,
                Message = e.Message
            });
        }
    }

    [HttpGet("[Action]")]
    [Authorize]
    public async Task<IActionResult> GetLoginInfo()
    {
        return Ok(new
        {
            TokenCode = User.FindFirstValue(JwtRegisteredClaimNames.Jti),
            Exp = User.FindFirstValue(JwtRegisteredClaimNames.Exp),
            userName = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            Roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value),
        });
    }
}