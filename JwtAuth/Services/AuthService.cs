using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public interface IAuthService
{
    Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord, HttpResponse? response = null);

    Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(string refreshToken);
}
public class AuthService : IAuthService
{
    public AuthService(AppDbContext dbContext, ITokenManagerService tokenManagerService)
    {
        this.dbContext = dbContext;
        this.tokenManagerService = tokenManagerService;
    }
    readonly AppDbContext dbContext;
    readonly ITokenManagerService tokenManagerService;

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord, HttpResponse? response = null)
    {
        var res = new ResponseBaseModel<AuthServiceModel_Login>();
        var user = await dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == userName
                                            && x.Password == passWord);
        if (user == null)
        {
            res.IsOk = false;
            res.StatusCode = 401;
            res.Message = "Sai tài khoản hoặc mật khẩu!";
            return res;
        }

        var data = new AuthServiceModel_Login
        {
            AccessToken = await tokenManagerService.GenerateAccessToken(user),
            RefreshToken = await tokenManagerService.GenerateRefreshToken()
        };

        await tokenManagerService.SaveRefreshToken(user,data.RefreshToken);

        if (response != null)
        {
            response.Cookies.Append("refresh_token",data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/Auth/RefreshSession",
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        res.IsOk = true;
        res.StatusCode = 200;
        res.Message = "Đăng nhập thành công";
        res.Data = data;
        return res;
    }

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(string refreshToken)
    {
        var res = new ResponseBaseModel<AuthServiceModel_Login>();
        var user = await tokenManagerService.VerifyRefreshToken(refreshToken);
        if (user == null)
        {
            res.StatusCode = 401;
            res.IsOk = false;
            res.Message = "Refresh token không khả dụng";
            return res;
        }

        var data = new AuthServiceModel_Login
        {
            AccessToken = await tokenManagerService.GenerateAccessToken(user),
            RefreshToken = await tokenManagerService.GenerateRefreshToken()
        };

        await tokenManagerService.SaveRefreshToken(user,data.RefreshToken);

        res.StatusCode = 200;
        res.IsOk = true;
        res.Message = "Làm mới phiên làm việc thành công";
        res.Data = data;
        return res;
    }
}