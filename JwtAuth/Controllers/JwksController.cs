using System.Security.Cryptography;
using JwtAuth.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Controllers;
[ApiController]
public class JwksController :ControllerBase
{
    public JwksController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    readonly AppDbContext dbContext;

    [HttpGet("~/.well-known/jwks.json")]
    public async Task<IActionResult> GetPublicKey()
    {
        var keysActive = await dbContext.JwtKeys.Where(x => x.IsActive && x.Exp > DateTime.Now).ToListAsync();
        return Ok(new
        {
            keys = keysActive.Select(x =>
            {
                var exponentAndModulus = GetExponentAndModulus(x.PublicKey);
                var modulus =  exponentAndModulus[0];
                var exponent = exponentAndModulus[1];
                return new
                {
                    kty = "RSA",
                    use = "sig",
                    kid = x.Code,
                    alg = SecurityAlgorithms.RsaSha256,
                    n = Base64UrlEncoder.Encode(modulus),
                    e = Base64UrlEncoder.Encode(exponent)
                };
            })
        });

    }

    private byte[][] GetExponentAndModulus(string publicKey)
    {
        using var rsa = RSA.Create();
        var keyBytes = Convert.FromBase64String(publicKey);
        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        var parameters = rsa.ExportParameters(false);
        if (parameters.Modulus == null)
        {
            throw new Exception("Khóa ngoại không có modulus. Vui lòng thử lại sau!");
        }
        if (parameters.Exponent == null)
        {
            throw new Exception("Khóa ngoại không có exponent. Vui lòng thử lại sau!");
        }
        return [
            parameters.Modulus,
            parameters.Exponent
        ];
    }
}