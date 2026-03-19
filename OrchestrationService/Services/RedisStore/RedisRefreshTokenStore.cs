using System.Text.Json;
using StackExchange.Redis;
using OrchestrationService.Model.Tokens;

namespace OrchestrationService.Services.RedisStore;

public class RedisRefreshTokenStore(IConnectionMultiplexer redis) : IRedisRefreshTokenStore
{
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task SaveAsync(RefreshToken token)
        {
                await _db.StringSetAsync(
                        key: $"refresh:{token.Token}",
                        value: JsonSerializer.Serialize(token),
                        expiry: token.ExpiresAt - DateTime.UtcNow
                );
        }

        public async Task<RefreshToken?> GetAsync(string token)
        {
                var value = await _db.StringGetAsync($"refresh:{token}");
                return value.HasValue
                        ? JsonSerializer.Deserialize<RefreshToken>(value!)
                        : null;
        }

        public async Task RevokeAsync(string token)
        {
                await _db.KeyDeleteAsync($"refresh:{token}");
        }
}