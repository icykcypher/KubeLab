using OrchestrationService.Model.Tokens;

namespace OrchestrationService.Services.RedisStore;

public interface IRedisRefreshTokenStore
{
        Task SaveAsync(RefreshToken token);
        Task<RefreshToken?> GetAsync(string token);
        Task RevokeAsync(string token);
}