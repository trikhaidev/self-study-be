using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public interface ITokenManagerService
{
    Task<string> GenerateAccessToken(User user, int? expires = null);

    Task<string> GenerateRefreshToken();

    Task SaveRefreshToken(User user, string refreshToken, int? expiresDate = null);
}
public class TokenManagerService : ITokenManagerService
{
    static RSA _rsa = RSA.Create();
    readonly AppDbContext dbContext;
    readonly JwtConfig jwtConfig;
    readonly HmacConfig hmacConfig;
    public TokenManagerService(AppDbContext dbContext, IOptions<JwtConfig> jwtConfigOptions, IOptions<HmacConfig> hmacConfigOptions)
    {
        this.dbContext = dbContext;
        this.jwtConfig = jwtConfigOptions.Value;
        this.hmacConfig = hmacConfigOptions.Value;
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
                           select r.Name).ToListAsync();
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

    public async Task<string> GenerateRefreshToken()
    {
        var buffer = new byte[32];
        var random = RandomNumberGenerator.Create();
        random.GetBytes(buffer);
        string refreshToken = Convert.ToBase64String(buffer);
        await Task.Delay(250);
        return refreshToken;
    }

    public async Task SaveRefreshToken(User user, string refreshToken, int? expiresDate = null)
    {
        var tokenExists = await dbContext.RefreshTokens
                                        .Where(x => x.UserId == user.Id && x.IsActive && x.Exp > DateTime.Now)
                                        .OrderByDescending(x => x.Exp)
                                        .ToListAsync();
        if (tokenExists.Count >= 3)
        {
            var deleteTokens = tokenExists.Skip(3).ToList();
            foreach (var item in deleteTokens)
            {
                item.IsActive = false;
            }
        }
        System.Console.WriteLine("Token raw: " + refreshToken);
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacConfig.refreshToken)!);
        var computedByte = hmac.ComputeHash(Convert.FromBase64String(refreshToken));
        var tokenHash = Convert.ToBase64String(computedByte);
        System.Console.WriteLine("Token hash: " + tokenHash);
        var data = new RefreshToken
        {
            UserId = user.Id,
            RefreshTokenHash = tokenHash,
            IsActive = true,
            Exp = DateTime.Now.AddDays(expiresDate ?? 3),
        };

        dbContext.Add(data);

        await dbContext.SaveChangesAsync();
    }
}