using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace JwtAuth.Middlewares;

public class AccessTokenBlockingMiddleware
{
    readonly RequestDelegate next;
    readonly IMemoryCache memoryCache;
    public AccessTokenBlockingMiddleware(RequestDelegate next, IMemoryCache memoryCache)
    {
        this.next = next;
        this.memoryCache = memoryCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
        {
            var token = authHeader.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync<ResponseBaseModel<object>>(new ResponseBaseModel<object>
                {
                    StatusCode = 401,
                    IsOk = false,
                    Message = "Vui lòng đăng nhập!"
                });
                return;
            }

            token = token.Substring("Bearer ".Length);
            if (memoryCache.TryGetValue(token, out _))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync<ResponseBaseModel<object>>(new ResponseBaseModel<object>
                {
                    StatusCode = 401,
                    IsOk = false,
                    Message = "Vui lòng đăng nhập!"
                });
                return;
            }
        }

        await next(context);
    }
}

public static class AccessTokenBlocking
{
    public static IApplicationBuilder UseAccessTokenBlocking(this WebApplication app)
    {
        return app.UseMiddleware<AccessTokenBlockingMiddleware>();
    }
}