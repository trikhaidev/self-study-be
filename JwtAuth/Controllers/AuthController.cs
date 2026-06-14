using JwtAuth.Services;
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
            var res = await authService.Login(model.UserName,model.Password);
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
}