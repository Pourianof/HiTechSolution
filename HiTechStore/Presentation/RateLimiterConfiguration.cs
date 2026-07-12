using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace HiTechStore.Presentation;

public static class RateLimiterConfiguration
{
    public static IServiceCollection WithRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                context =>
                {
                    IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (remoteIpAddress is null)
                    {
                        // Test Env
                        return RateLimitPartition.GetNoLimiter("Test");
                    }

                    if (!IPAddress.IsLoopback(remoteIpAddress!))
                    {
                        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
                        return RateLimitPartition.GetTokenBucketLimiter(
                            partitionKey: isAuthenticated ? $"user:{context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}" : $"guest:{remoteIpAddress}",
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = isAuthenticated ? 500 : 100,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(3),
                                TokensPerPeriod = 1,
                            });
                    }

                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback.ToString());
                }
            );
        });


        return services;
    }
}