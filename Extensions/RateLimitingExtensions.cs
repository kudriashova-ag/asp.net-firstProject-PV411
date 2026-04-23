using System.Security.Claims;
using System.Threading.RateLimiting;

namespace MyApp.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingConfiguration(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(

                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(1),
                        PermitLimit = 10,
                        QueueLimit = 0
                    })
                ),
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var key = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? context.Connection.RemoteIpAddress?.ToString()
                              ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(10),
                        PermitLimit = 5,
                        QueueLimit = 0
                    });
                })
            );

            options.AddPolicy("auth", context =>
            {
                var key = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 5,
                    QueueLimit = 0
                });
            });

            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later."
                }, ct);
            };
        });

        return services;
    }
}