namespace OrchestrationService.Model.Tokens;

public class AccessToken
{
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string UserId { get; init; } = string.Empty;
        public int Role { get; init; }
}