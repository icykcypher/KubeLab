using OrchestrationService.Model.Dtos;
using OrchestrationService.Model.Requests;
using OrchestrationService.Model.Tokens;
using OrchestrationService.Shared;

namespace OrchestrationService.Services.Authorization;

public interface IUserService
{
        Task<Result<TokensDto>> Login(LoginRequest loginRequest);
        Task<Result<AccessToken>> Register(RegisterRequest request, string role = "student");
        Task<string> GenerateUniquePublicId();
}