using JwtAuthentication.Database;

namespace JwtAuthentication.Services;
public class AutoChangeJwtKeyService:BackgroundService
{
    readonly IServiceProvider services;
    public AutoChangeJwtKeyService(IServiceProvider services)
    {
        this.services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            
        }while(!stoppingToken.CanBeCanceled);
    }

    private async Task AutoChangeJwtKey(CancellationToken stoppingToken)
    {
        var scoped = services.CreateScope();
        var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}