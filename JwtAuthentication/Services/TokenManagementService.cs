using JwtAuthentication.Database;
using Microsoft.Extensions.Options;
using JwtAuthentication.Models.Request;
using JwtAuthentication.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using JwtAuthentication.Database.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace JwtAuthentication.Services;

public interface ITokenManagementService
{
    Task<BaseResponseModel<LoginResponse>> Login(LoginRequest request);
    Task<BaseResponseModel<string>> GenerateAccessToken(AppUser user);
    BaseResponseModel<string> GenerateRefreshToken();
    Task<BaseResponseModel<string>> SaveToken(AppUser user, string accessToken, string refreshToken);
    Task<BaseResponseModel<LoginResponse>> RefreshSession(string refreshToken);
    Task<BaseResponseModel<string>> Logout(string accessToken);
}

public class TokenManagementService : ITokenManagementService
{
    static RSA _rsa = RSA.Create();
    readonly AppDbContext context;
    readonly JwtConfig jwtConfig;
    readonly IMemoryCache cache;
    public TokenManagementService(AppDbContext context, IMemoryCache cache,IOptions<JwtConfig> options)
    {
        this.context = context;
        this.jwtConfig = options.Value;
        this.cache = cache;
    }
    public async Task<BaseResponseModel<LoginResponse>> Login(LoginRequest request)
    {
        var res = new BaseResponseModel<LoginResponse>();
        try
        {
            var user = await context.Users.AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.UserName == request.UserName
                                                                && x.Password == request.Password);
            if (user == null)
            {
                res.Message = "Sai tên tài khoản hoặc mật khẩu!";
                return res;
            }
            var accessToken = await GenerateAccessToken(user);
            if (accessToken.IsError)
            {
                res.IsError = true;
                res.Message = "Vui lòng thử lại sau!";
                return res;
            }
            var refreshToken = GenerateRefreshToken();

            var saveToken = await SaveToken(user, accessToken.Data!, refreshToken.Data!);
            if (saveToken.IsError)
            {
                res.IsError = true;
                res.Message = "Vui lòng thử lại sau!";
                return res;
            }

            res.Data = new LoginResponse
            {
                AccessToken = accessToken.Data!,
                RefreshToken = refreshToken.Data!,
            };
            res.Message = "Đăng nhập thành công";
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }

    public async Task<BaseResponseModel<string>> SaveToken(AppUser user, string accessToken, string refreshToken)
    {
        var res = new BaseResponseModel<string>();
        try
        {
            var tokensActive = await context.UserTokens.Where(x => x.UserId == user.Id
                                                                    && x.IsActive)
                                                        .OrderByDescending(x => x.Exp)
                                                        .ToListAsync();
            if (tokensActive.Count() > 2)
            {
                tokensActive.Skip(2).ToList().ForEach(x =>
                {
                    x.IsActive = false;
                });
            }

            var data = new AppUserToken
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                UserId = user.Id,
                IsActive = true,
                Exp = DateTime.Now.AddDays(1),
            };

            context.Add(data);
            await context.SaveChangesAsync();

            res.Message = "Lưu token thành công";
            res.Data = string.Empty;
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }

    public async Task<BaseResponseModel<string>> GenerateAccessToken(AppUser user)
    {
        var res = new BaseResponseModel<string>();
        try
        {
            var key = await context.JwtKeys.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive);
            if (key == null)
            {
                res.IsError = true;
                res.Message = "Vui lòng thử lại sau";
                return res;
            }

            _rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(key.PrivateKey), out _);
            var rsaSecurityKey = new RsaSecurityKey(_rsa)
            {
                KeyId = key.KeyId,
            };

            var signInCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await (from ur in context.UserRoles
                               join r in context.Roles on ur.RoleId equals r.Id
                               where ur.UserId == user.Id
                               select r.Name).ToListAsync();

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                expires: DateTime.Now.AddMinutes(jwtConfig.ExpMinutes),
                claims: claims,
                signingCredentials: signInCredentials
            );
            res.Message = "Đăng nhập thành công";
            res.Data = new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }

    public BaseResponseModel<string> GenerateRefreshToken()
    {
        var res = new BaseResponseModel<string>();
        try
        {
            var buffer = new byte[32];
            var random = System.Security.Cryptography.RandomNumberGenerator.Create();
            random.GetBytes(buffer);

            res.Data = Convert.ToBase64String(buffer);
            res.Message = "Tạo refresh token thành công";
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }

    public async Task<BaseResponseModel<LoginResponse>> RefreshSession(string refreshToken)
    {
        var res = new BaseResponseModel<LoginResponse>();
        try
        {
            var tokenWithUser = await context.UserTokens.Include(x => x.User)
                                                        .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
                                                                            && x.IsActive
                                                                            && x.Exp > DateTime.Now);
            if (tokenWithUser == null)
            {
                res.Message = "Mã làm mới phiên đăng nhập không khả dụng!";
                return res;
            }

            tokenWithUser.IsActive = false;
            await context.SaveChangesAsync();
            var logout = await Logout($"Bearer {tokenWithUser.AccessToken}");
            if (logout.IsError)
            {
                res.Message = "Vui lòng thử lại sau!";
                return res;
            }
            return await Login(new LoginRequest
            {
                UserName = tokenWithUser.User!.UserName,
                Password = tokenWithUser.User!.Password,
            });
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }

    public async Task<BaseResponseModel<string>> Logout(string accessToken)
    {
        var res = new BaseResponseModel<string>();
        try
        {
            if (!cache.TryGetValue(accessToken, out _))
            {
                var options = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                            .SetPriority(CacheItemPriority.High);
                cache.Set(accessToken,true,options);
            }

            var userToken = await context.UserTokens.FirstOrDefaultAsync(x => x.AccessToken == accessToken.Substring("Bearer ".Length));
            if (userToken != null)
            {
                userToken.IsActive = false;
                await context.SaveChangesAsync();
            }

            res.Message = "Đăng xuất thành công";
        }
        catch (Exception e)
        {
            res.IsError = true;
            res.Message = $"Lỗi: {e.Message}";
        }
        return res;
    }
}

public class JwtConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpMinutes { get; set; }
    public string IssuerHost { get; set; } = null!;
}