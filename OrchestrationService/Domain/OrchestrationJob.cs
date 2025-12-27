using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Domain;

public sealed class OrchestrationJob
{
        [BsonId]
        public ObjectId Id { get; init; }
        public ObjectId NamespaceId { get; init; }
        public OrchestrationAction Action { get; init; }
        public ObjectId TargetId { get; init; }
        public JobStatus Status { get; set; }
        public string? Error { get; set; }
        public DateTime CreatedAt { get; init; }
}