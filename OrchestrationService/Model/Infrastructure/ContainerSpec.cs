namespace OrchestrationService.Model.Infrastructure;

public sealed class ContainerSpec
{
        public string Image { get; init; } = default!;
        public List<int> Ports { get; init; } = [];
}