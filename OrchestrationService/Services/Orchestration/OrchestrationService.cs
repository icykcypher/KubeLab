using OrchestrationService.Domain;
using OrchestrationService.Shared;
using OrchestrationService.Repositories;
using OrchestrationService.AsyncDataServices;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Services.Orchestration;

public class OrchestrationService(
    IOrchestrationJobRepository jobRepository,
    IOrchestrationPublisher publisher,
    IUserRepository userRepository) : IOrchestrationService
{
    private readonly IOrchestrationJobRepository _jobRepository = jobRepository;
    private readonly IOrchestrationPublisher _publisher = publisher;

    public async Task<Result<Guid>> StartScenarioAsync(string studentPublicId, string scenarioId)
    {
        var user = await userRepository.GetUserByPublicId(studentPublicId);
        if (user == null)
            return Result<Guid>.Failure("Student not found", ErrorStatus.NotFound);

        var job = new OrchestrationJob
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId(),
            NamespaceId = user.Id,
            TargetId = MongoDB.Bson.ObjectId.GenerateNewId(),
            Action = OrchestrationAction.DeployRevision,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _jobRepository.AddJobAsync(job);

        var message = new
        {
            JobId = job.Id.ToString(),
            Student = user.PublicId,
            Scenario = scenarioId
        };

        await _publisher.PublishAsync(message);

        return Result<Guid>.Success(Guid.Parse(job.Id.ToString()));
    }

    public async Task<Result<bool>> DeleteScenarioAsync(string studentPublicId, string scenarioId)
    {
        var user = await userRepository.GetUserByPublicId(studentPublicId);
        if (user == null)
            return Result<bool>.Failure("Student not found", ErrorStatus.NotFound);

        var job = new OrchestrationJob
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId(),
            NamespaceId = user.Id,
            TargetId = MongoDB.Bson.ObjectId.GenerateNewId(),
            Action = OrchestrationAction.DeleteService,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _jobRepository.AddJobAsync(job);

        var message = new
        {
            JobId = job.Id.ToString(),
            Student = user.PublicId,
            Scenario = scenarioId,
            Action = "delete"
        };

        await _publisher.PublishAsync(message);

        return Result<bool>.Success(true);
    }
}