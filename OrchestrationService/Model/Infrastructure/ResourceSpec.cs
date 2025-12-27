namespace OrchestrationService.Model.Infrastructure;

public sealed class ResourceSpec
{
        public string Cpu { get; init; } = default!;
        public string Memory { get; init; } = default!;
}