using System.Text.Json;
using CargoTracker.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace CargoTracker.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        return json is null ? null : JsonSerializer.Deserialize<T>(json, jsonOptions);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(key, cancellationToken);


    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        var json = JsonSerializer.Serialize(value, jsonOptions);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        }, cancellationToken);
    }
}
