
using InMemoryCachingDemo.Database;
using InMemoryCachingDemo.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace InMemoryCachingDemo
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
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddSingleton<ICacheManagerService, CacheManagerService>();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("RedisOptions").GetSection("Configuration").Get<string>();
                options.InstanceName = builder.Configuration.GetSection("RedisOptions").GetSection("InstanceName").Get<string>();
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(builder.Configuration.GetSection("RedisOptions").GetSection("Configuration").Value!));
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
