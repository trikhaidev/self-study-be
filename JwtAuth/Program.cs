
using JwtAuth.BackgroundServices;
using JwtAuth.Database;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions();
        builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

        // Add services to the container.
        builder.Services.AddScoped<IAuthService,AuthService>();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"));
        });
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddHostedService<KeyRotationService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            JwtConfig jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>()!;
                            options.TokenValidationParameters = new()
                            {
                                ValidateIssuer = true, // Đảm bảo nhà cung cấp token hợp lệ
                                ValidIssuer = jwtConfig.Issuer, // Thông tin nhà cung cấp token hợp lệ

                                RequireAudience = true, // Đảm bảo phải có thông tin client nhận
                                ValidateAudience = true, // Client nhận phải hợp lệ
                                ValidAudience = jwtConfig.Audience, // thông tin client hợp lệ
                                
                                RequireExpirationTime = true, // Đảm bảo token phải có hạn sử dụng
                                ValidateLifetime = true, // Token phải còn hạn sử dụng
                                ClockSkew = TimeSpan.Zero, // không cho phép độ trễ

                                RequireSignedTokens = true, // Đảm bảo phải được kí xác thực

                                // Sử lấy thông tin xác thức chữ kí token
                                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                                {
                                    using var httpClinet = new HttpClient();
                                    var res = httpClinet.GetAsync($"{jwtConfig.Issuer}/.well-known/jwks.json").Result;
                                    var content = res.Content.ReadAsStringAsync().Result;
                                    var jwks = new JsonWebKeySet(content);
                                    return jwks.Keys;
                                }
                            };
                        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
