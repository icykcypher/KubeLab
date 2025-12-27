namespace OrchestrationService.Model.Infrastructure;

public sealed class ScenarioStep
{
        public StepAction Action { get; init; }
        public string Service { get; init; } = default!;
        public string? Image { get; init; }
        public UpdateStrategy? Strategy { get; init; }
}