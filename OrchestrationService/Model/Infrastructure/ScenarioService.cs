namespace OrchestrationService.Model.Infrastructure;

public class ScenarioService
{
        public string Name { get; init; } = default!;
        public ServiceKind Kind { get; init; }
        public ContainerSpec Initial { get; init; } = default!;
}