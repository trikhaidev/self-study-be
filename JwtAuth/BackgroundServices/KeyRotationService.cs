using System.Security.Cryptography;
using JwtAuth.Database;
using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.BackgroundServices;

public class KeyRotationService : BackgroundService
{
    readonly IServiceProvider serviceProvider;
    public KeyRotationService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RotateKey();
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task RotateKey()
    {
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var currentKey = await dbContext.JwtKeys.FirstOrDefaultAsync(x => x.IsActive);
        if (currentKey == null || currentKey.Exp <= DateTime.Now.AddMinutes(10))
        {
            if (currentKey != null)
            {
                currentKey.IsActive = false;
            }

            using var rsa = RSA.Create(2048);
            _privateKey = rsa.ExportPkcs8PrivateKey();
            var publicKeyByte = rsa.ExportSubjectPublicKeyInfo();
            _keyId = Guid.NewGuid().ToString();
            var newJwtKey = new JwtKey
            {
                Code = _keyId,
                PublicKey = Convert.ToBase64String(publicKeyByte),
                IsActive = true,
                Exp = DateTime.Now.AddMinutes(40)
            };

            dbContext.Add(newJwtKey);
            await dbContext.SaveChangesAsync();
        }
    }

    private static string _keyId = null!;
    public static string KeyId { get => _keyId; }
    
    private static byte[] _privateKey = null!;
    public static byte[] PrivateKey { get => _privateKey; }
}