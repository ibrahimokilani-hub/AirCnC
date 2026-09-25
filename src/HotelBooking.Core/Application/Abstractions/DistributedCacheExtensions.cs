using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HotelBooking.Core.Application.Abstractions;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static async Task<T> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        TimeSpan ttl,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(key, cancellationToken);
        if (cached is not null)
        {
            var hit = JsonSerializer.Deserialize<T>(cached, Json);
            if (hit is not null)
            {
                return hit;
            }
        }

        var value = await factory(cancellationToken);

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value, Json),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
            cancellationToken);

        return value;
    }
}