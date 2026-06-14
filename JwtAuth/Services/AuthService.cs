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
    Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord);
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

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord)
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

        res.IsOk = true;
        res.StatusCode = 200;
        res.Message = "Đăng nhập thành công";
        res.Data = data;
        return res;
    }
}