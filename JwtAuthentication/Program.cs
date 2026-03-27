
using JwtAuthentication.Database;
using JwtAuthentication.Middlewares;
using JwtAuthentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthentication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings").GetSection("AppDbContext").Get<string>());
        });

        // Đăng kí AutoChangeJwtKeyService làm service chạy nên. Sẽ định kì thay đổi key xác thực
        builder.Services.AddHostedService<AutoChangeJwtKeyService>();
        
        builder.Services.AddOptions();
        builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddScoped<ITokenManagementService, TokenManagementService>();

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        using var httpClient = new HttpClient();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;
                            options.TokenValidationParameters = new()
                            {
                                ValidateIssuer = true,
                                RequireAudience = true,
                                ValidateAudience = true,
                                RequireExpirationTime = true,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero,
                                RequireSignedTokens = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtConfig.Issuer,
                                ValidAudience = jwtConfig.Audience,
                                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                                {
                                    var res = httpClient.GetAsync($"{jwtConfig.IssuerHost}/JWKS/jwks.json").Result;
                                    var jwks = new JsonWebKeySet(res.Content.ReadAsStringAsync().Result);
                                    return jwks.Keys;
                                }
                            };
                        });
        builder.Services.AddMemoryCache();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<CheckLogOutMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
