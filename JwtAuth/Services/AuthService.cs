using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public interface IAuthService
{
    Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord, HttpResponse? response = null);

    Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(string refreshToken, HttpResponse? response = null);

    Task<ResponseBaseModel<string>> Logout(HttpRequest request);

    void SetRefreshTokenCookie(string refreshToken, HttpResponse response);
}
public class AuthService : IAuthService
{
    public AuthService(AppDbContext dbContext, ITokenManagerService tokenManagerService, IMemoryCache memoryCache, IOptions<JwtConfig> jwtConfigOptions)
    {
        this.dbContext = dbContext;
        this.tokenManagerService = tokenManagerService;
        this.memoryCache = memoryCache;
        this.jwtConfig = jwtConfigOptions.Value;
    }
    readonly AppDbContext dbContext;
    readonly ITokenManagerService tokenManagerService;
    readonly IMemoryCache memoryCache;
    readonly JwtConfig jwtConfig;

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

        await tokenManagerService.SaveRefreshToken(user, data.RefreshToken);

        if (response != null)
        {
            SetRefreshTokenCookie(data.RefreshToken, response);
        }

        res.IsOk = true;
        res.StatusCode = 200;
        res.Message = "Đăng nhập thành công";
        res.Data = data;
        return res;
    }

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(string refreshToken, HttpResponse? response)
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

        await tokenManagerService.SaveRefreshToken(user, data.RefreshToken);

        if (response != null)
        {
            SetRefreshTokenCookie(data.RefreshToken, response);
        }

        res.StatusCode = 200;
        res.IsOk = true;
        res.Message = "Làm mới phiên làm việc thành công";
        res.Data = data;
        return res;
    }

    public async Task<ResponseBaseModel<string>> Logout(HttpRequest request)
    {
        var res = new ResponseBaseModel<string>();
        if (!request.Headers.TryGetValue("Authorization", out StringValues authHeader))
        {
            res.StatusCode = 401;
            res.IsOk = false;
            res.Message = "Vui lòng đăng nhập!";
            return res;
        }

        var token = authHeader.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(token))
        {
            res.StatusCode = 401;
            res.IsOk = false;
            res.Message = "Vui lòng đăng nhập!";
            return res;
        }

        token = token.Substring("Bearer ".Length);
        memoryCache.Set(token, true, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(jwtConfig.Expires),
            Priority = CacheItemPriority.High
        });

        if (request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await tokenManagerService.DeleteRefreshToken(refreshToken);
            }
        }

        res.StatusCode = 200;
        res.IsOk = true;
        res.Message = "Đăng xuất thành công";
        return res;
    }

    public void SetRefreshTokenCookie(string refreshToken, HttpResponse response)
    {
        response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/Auth/Session"
        });
    }
}