using JwtAuthentication.Models.Response;
using Microsoft.Extensions.Caching.Memory;

namespace JwtAuthentication.Middlewares;
public class CheckLogOutMiddleware
{
    readonly IMemoryCache cache;
    readonly RequestDelegate next;
    public CheckLogOutMiddleware(IMemoryCache cache, RequestDelegate next)
    {
        this.cache = cache;
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            var token = authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new BaseResponseModel<string>
                {
                    Message = "Accesstoken không hợp lệ!"
                });
                return;
            }
            if (cache.TryGetValue(token!,out _))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new BaseResponseModel<string>
                {
                    Message = "Accesstoken không khả dụng!"
                });
                return;
            }
        }
        await next(context);
    }
}