
using Microsoft.EntityFrameworkCore;
using RedisCachingDemo.Database;
using RedisCachingDemo.Services;
using StackExchange.Redis;
using static RedisCachingDemo.Services.IRedisCache;

namespace RedisCachingDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"));
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var config = builder.Configuration.GetSection("RedisConfig").Get<RedisConfig>()!;
                options.Configuration = config.Host;
                options.InstanceName = config.InstanceName;
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(options => ConnectionMultiplexer.Connect(builder.Configuration.GetSection("RedisConfig").GetSection("Host").Value!));
            builder.Services.AddScoped<IRedisCache, RedisCache>();
            builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("RedisConfig"));
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any",policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("any");
            
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
