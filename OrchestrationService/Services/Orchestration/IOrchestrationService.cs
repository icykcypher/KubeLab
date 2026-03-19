using OrchestrationService.Shared;

namespace OrchestrationService.Services.Orchestration;

public interface IOrchestrationService
{
        Task<Result<Guid>> StartScenarioAsync(string studentPublicId, string scenarioId);
        Task<Result<bool>> DeleteScenarioAsync(string studentPublicId, string scenarioId);
}