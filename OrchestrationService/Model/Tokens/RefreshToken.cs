namespace OrchestrationService.Model.Tokens;

public class RefreshToken
{
        public string Token { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public bool IsRevoked { get; set; }
}