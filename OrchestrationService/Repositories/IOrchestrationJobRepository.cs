using OrchestrationService.Domain;

namespace OrchestrationService.Repositories;

public interface IOrchestrationJobRepository
{
        Task AddJobAsync(OrchestrationJob job);
        Task<OrchestrationJob?> GetJobByIdAsync(MongoDB.Bson.ObjectId id);
}