using System.Security.Claims;
using System.Security.Cryptography;
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;
public class AuthService
{
    static RSA _rsa = RSA.Create();
    readonly AppDbContext dbContext;
    public AuthService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        if (KeyRotationService.PrivateKey == null)
        {
            throw new Exception("Can not find private key");
        }

        _rsa.ImportPkcs8PrivateKey(KeyRotationService.PrivateKey, out _);
        var securityKey = new RsaSecurityKey(_rsa)
        {
            KeyId = KeyRotationService.KeyId
        };
        var signCredential = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username)
        };
        return string.Empty;
    }
}