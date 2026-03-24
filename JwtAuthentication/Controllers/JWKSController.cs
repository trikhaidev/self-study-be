using System.Security.Cryptography;
using JwtAuthentication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthentication.Controllers;
[ApiController]
[Route("[Controller]")]
public class JWKSController : ControllerBase
{
    readonly AppDbContext context;
    public JWKSController(AppDbContext context)
    {
        this.context = context;
    }

    [HttpGet("jwks.json")]
    public async Task<IActionResult> GetJWKS()
    {
        var keys = await context.JwtKeys.Where(x => x.IsActive)
                                        .AsNoTracking()
                                        .ToListAsync();
        return Ok(new
        {
            Keys = keys.Select(x => new
            {
                kty = "RSA",
                use = "sig",
                kid = x.KeyId,
                alg = SecurityAlgorithms.RsaSha256,
                n = Base64UrlEncoder.Encode(GetModulus(x.PublicKey)),
                e = Base64UrlEncoder.Encode(GetExponent(x.PublicKey))
            })
        });
    }

    private byte[] GetModulus(string publicKey)
    {
        var bytes = Convert.FromBase64String(publicKey);
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(bytes, out _);
        var param = rsa.ExportParameters(false);
        if (param.Modulus == null)
        {
            throw new Exception("Lỗi không có modulus");
        }
        return param.Modulus;
    }

    private byte[] GetExponent(string publicKey)
    {
        var bytes = Convert.FromBase64String(publicKey);
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(bytes, out _);
        var param = rsa.ExportParameters(false);
        if (param.Exponent == null)
        {
            throw new Exception("Lỗi không có exponent");
        }
        return param.Exponent;
    }
}