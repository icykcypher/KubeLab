using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Model.Domain;

public sealed class ScenarioDefinition
{
        [BsonId]
        public ObjectId Id { get; init; }

        public string ScenarioId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        
        public List<ScenarioService> Services { get; init; } = new();
        public List<ScenarioStep> Steps { get; init; } = new();
}