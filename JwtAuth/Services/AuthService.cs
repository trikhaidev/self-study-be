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
    Task<string> GenerateAccessToken(User user, int? expires = null);
}
public class AuthService : IAuthService
{
    static RSA _rsa = RSA.Create();
    public AuthService(AppDbContext dbContext, IOptions<JwtConfig> options)
    {
        this.dbContext = dbContext;
        jwtConfig = options.Value;
    }
    readonly AppDbContext dbContext;
    readonly JwtConfig jwtConfig;

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
            AccessToken = await GenerateAccessToken(user)
        };

        res.IsOk = true;
        res.StatusCode = 200;
        res.Message = "Đăng nhập thành công";
        res.Data = data;
        return res;
    }

    public async Task<string> GenerateAccessToken(User user, int? expires = null)
    {
        if (String.IsNullOrWhiteSpace(KeyRotationService.KeyId) || KeyRotationService.PrivateKey == null || KeyRotationService.PrivateKey.Length == 0)
        {
            throw new Exception("Không có khóa để tạo token");
        }

        _rsa.ImportPkcs8PrivateKey(KeyRotationService.PrivateKey, out _);
        var securityKey = new RsaSecurityKey(_rsa)
        {
            KeyId = KeyRotationService.KeyId
        };
        var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email??""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };
        var roles = await (from ur in dbContext.UserRoles
                           join r in dbContext.Roles on ur.RoleId equals r.Id
                           where ur.UserId == user.Id
                           select r.Name)
                        .ToListAsync();
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expires ?? jwtConfig.Expires),
            signingCredentials: signingCredential
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}