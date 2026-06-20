using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using JwtAuth.Services.options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public interface IAuthService
{
    Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord, HttpResponse? response = null);

    Task<ResponseBaseModel<int>> Register(AuthRequestModel_Register request);

    Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(HttpRequest request, HttpResponse? response = null);

    Task<ResponseBaseModel<string>> Logout(HttpRequest request, HttpResponse response);

    void SetRefreshTokenCookie(string refreshToken, HttpResponse response);
}
public class AuthService : IAuthService
{
    public AuthService(AppDbContext dbContext,
        ITokenManagerService tokenManagerService,
        IMemoryCache memoryCache,
        IOptions<JwtConfig> jwtConfigOptions,
        IOptions<RefreshTokenConfig> refreshTokenConfigOptions,
        IOptions<HmacConfig> hmaConfigOptions)
    {
        this.dbContext = dbContext;
        this.tokenManagerService = tokenManagerService;
        this.memoryCache = memoryCache;
        this.jwtConfig = jwtConfigOptions.Value;
        this.refreshTokenConfig = refreshTokenConfigOptions.Value;
        this.hmacConfig = hmaConfigOptions.Value;
    }
    readonly AppDbContext dbContext;
    readonly ITokenManagerService tokenManagerService;
    readonly IMemoryCache memoryCache;
    readonly JwtConfig jwtConfig;
    readonly RefreshTokenConfig refreshTokenConfig;
    readonly HmacConfig hmacConfig;

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> Login(string userName, string passWord, HttpResponse? response = null)
    {
        var res = new ResponseBaseModel<AuthServiceModel_Login>();
        var hashPasword = GlobalStaticService.HashData(passWord, hmacConfig.password);
        var user = await dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == userName
                                            && x.Password == hashPasword);
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

    public async Task<ResponseBaseModel<int>> Register(AuthRequestModel_Register request)
    {
        var res = new ResponseBaseModel<int>();
        request.Username = request.Username.Trim();
        request.Password = request.Password.Trim();
        request.Email = request.Email?.Trim();
        if (await dbContext.Users.AnyAsync(x => x.Username == request.Username))
        {
            res.StatusCode = 400;
            res.Message = "Username đã tồn tại!";
            return res;
        }

        var data = new User
        {
            Username = request.Username,
            Password = GlobalStaticService.HashData(request.Password, hmacConfig.password),
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };

        dbContext.Add(data);

        res.Data = await dbContext.SaveChangesAsync();
        res.StatusCode = 201;
        res.IsOk = true;
        res.Message = "Đăng kí thành công";
        return res;
    }

    public async Task<ResponseBaseModel<AuthServiceModel_Login>> RefreshSession(HttpRequest request, HttpResponse? response)
    {
        var res = new ResponseBaseModel<AuthServiceModel_Login>();

        if (!request.Cookies.TryGetValue(refreshTokenConfig.Key, out string? refreshToken)
                || string.IsNullOrWhiteSpace(refreshToken))
        {
            res.StatusCode = 401;
            res.IsOk = false;
            res.Message = "Phiên làm việc hết hạn! Vui lòng đăng nhập lại!";
            return res;
        }

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

    public async Task<ResponseBaseModel<string>> Logout(HttpRequest request, HttpResponse response)
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

        if (request.Cookies.TryGetValue(refreshTokenConfig.Key, out var refreshToken))
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await tokenManagerService.DeleteRefreshToken(refreshToken);
            }

            response.Cookies.Delete(refreshTokenConfig.Key, new CookieOptions
            {
                Path = refreshTokenConfig.Path
            });
        }

        res.StatusCode = 200;
        res.IsOk = true;
        res.Message = "Đăng xuất thành công";
        return res;
    }

    public void SetRefreshTokenCookie(string refreshToken, HttpResponse response)
    {
        response.Cookies.Append(refreshTokenConfig.Key, refreshToken, new CookieOptions
        {
            HttpOnly = refreshTokenConfig.HttpOnly,
            Secure = refreshTokenConfig.Secure,
            SameSite = (SameSiteMode)refreshTokenConfig.SameSite,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenConfig.ExpiressDay),
            Path = refreshTokenConfig.Path
        });
    }
}