
using System.Security.Cryptography;
using JwtAuthentication.Database;
using JwtAuthentication.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Services;
public class AutoChangeJwtKeyService : BackgroundService
{
    readonly IServiceProvider service;
    public AutoChangeJwtKeyService(IServiceProvider service)
    {
        this.service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            await AutoChangeKey(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5),stoppingToken);
        }while(!stoppingToken.IsCancellationRequested);
    }

    private async Task AutoChangeKey(CancellationToken stoppingToken)
    {
        using var scoped = service.CreateScope();
        using var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
        var currentKey = await context.JwtKeys.FirstOrDefaultAsync(x => x.IsActive,stoppingToken);
        if (currentKey == null || currentKey.Exp <= DateTime.Now.AddMinutes(10))
        {
            if (currentKey != null)
            {
                currentKey.IsActive = false;
            }

            using var rsa = RSA.Create(2048);
            var newKey = new AppJwtKey
            {
                KeyId = Guid.NewGuid().ToString(),
                PublicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                PrivateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()),
                Exp = DateTime.Now.AddMinutes(30),
                IsActive = true,
            };
            context.Add(newKey);
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}