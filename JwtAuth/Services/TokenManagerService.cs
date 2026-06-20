using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public interface ITokenManagerService
{
    Task<string> GenerateAccessToken(User user, int? expires = null);

    Task<string> GenerateRefreshToken();

    Task SaveRefreshToken(User user, string refreshToken, int? expiresDate = null);

    Task<int> DeleteRefreshToken(string refreshToken);

    Task<User?> VerifyRefreshToken(string refreshToken);
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
        var tokenHash = GlobalStaticService.HashData(refreshToken,hmacConfig.refreshToken);
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

    public async Task<int> DeleteRefreshToken(string refreshToken)
    {
        var tokenHash = GlobalStaticService.HashData(refreshToken,hmacConfig.refreshToken);
        var tokenExis = await dbContext.RefreshTokens.Where(x => x.RefreshTokenHash == tokenHash && x.IsActive)
                            .ToListAsync();
        foreach (var item in tokenExis)
        {
            item.IsActive = false;
        }

        return await dbContext.SaveChangesAsync();
    }

    public async Task<User?> VerifyRefreshToken(string refreshToken)
    {
        var hashRefreshToken = GlobalStaticService.HashData(refreshToken,hmacConfig.refreshToken);
        var query = from rf in dbContext.RefreshTokens
                    join u in dbContext.Users on rf.UserId equals u.Id
                    where rf.IsActive
                    && rf.Exp > DateTime.Now
                    && rf.RefreshTokenHash == hashRefreshToken
                    select u;
        return await query.FirstOrDefaultAsync();
    }
}