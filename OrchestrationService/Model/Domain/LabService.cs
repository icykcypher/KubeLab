using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Model.Domain;

public sealed class LabService
{
        [BsonId]
        public ObjectId Id { get; init; }
        public ObjectId NamespaceId { get; init; }

        public string Name { get; init; } = default!;
        public ServiceKind Kind { get; init; }
        public int CurrentRevision { get; set; }
        public bool IsActive { get; set; }
}