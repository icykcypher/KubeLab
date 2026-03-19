using OrchestrationService.Domain;
using OrchestrationService.Model.Tokens;

namespace OrchestrationService.Services.TokenProvider;

public interface ITokenProvider
{
        AccessToken CreateAccessToken(User user);
        RefreshToken CreateRefreshToken(User user);
}