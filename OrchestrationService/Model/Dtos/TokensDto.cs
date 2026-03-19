using OrchestrationService.Model.Tokens;

namespace OrchestrationService.Model.Dtos;

public class TokensDto
{
        public AccessToken AccessToken { get; set; } = null!;
        public RefreshToken RefreshToken { get; set; } =  null!;
}