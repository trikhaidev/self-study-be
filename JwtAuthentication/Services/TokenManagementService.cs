using JwtAuthentication.Database;
using Microsoft.Extensions.Options;
using JwtAuthentication.Models.Request;
using JwtAuthentication.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace JwtAuthentication.Services;

public interface ITokenManagementService
{
    Task<BaseResponseModel<string>> GenerateAccessToken(LoginRequest request);
}

public class TokenManagementService:ITokenManagementService
{
    static RSA _rsa = RSA.Create();
    readonly AppDbContext context;
    readonly JwtConfig jwtConfig;
    public TokenManagementService(AppDbContext context, IOptions<JwtConfig> options)
    {
        this.context = context;
        this.jwtConfig = options.Value;
    }
    public async Task<BaseResponseModel<string>> GenerateAccessToken(LoginRequest request)
    {
        var res = new BaseResponseModel<string>();
        try
        {
            var user = await context.Users.AsNoTracking()
                                            .FirstOrDefaultAsync(x => x.UserName == request.UserName && x.Password == request.Password);
            if (user == null)
            {
                res.Message = "Sai tài khoản hoặc mật khẩu";
                return res;
            }

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

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role,x)));

            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience:jwtConfig.Audience,
                expires: DateTime.Now.AddMinutes(jwtConfig.ExpMinutes),
                claims:claims,
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

    
}

public class JwtConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpMinutes {get;set;}
    public string IssuerHost {get;set;} = null!;
}