namespace OrchestrationService.AsyncDataServices;

public interface IOrchestrationPublisher
{
        Task PublishAsync(object message);
        void Dispose();
}