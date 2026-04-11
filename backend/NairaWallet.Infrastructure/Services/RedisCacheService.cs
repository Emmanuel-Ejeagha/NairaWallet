namespace NairaWallet.Infrastructure.Services;

/// <summary>
/// Redis cache service for idempotency and rate limiting.
/// </summary>
public class RedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiry)
    {
        var db = _redis.GetDatabase();
        return await db.StringSetAsync(key, value, expiry, When.NotExists);
    }

    public async Task<string?> GetAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task IncrementAsync(string key, TimeSpan expiry)
    {
        var db = _redis.GetDatabase();
        await db.StringIncrementAsync(key);
        await db.KeyExpireAsync(key, expiry);
    }
}