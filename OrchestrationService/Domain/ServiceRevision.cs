using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Domain;

public sealed class ServiceRevision
{
        [BsonId]
        public ObjectId Id { get; init; }
        public ObjectId ServiceId { get; init; }
        public int Revision { get; init; }

        public ContainerSpec Container { get; init; } = default!;
        public ReplicaSpec Replicas { get; init; } = default!;
        public ResourceSpec Resources { get; init; } = default!;
        public Dictionary<string, string> Labels { get; init; } = [];

        public RevisionStatus Status { get; set; }
        public DateTime CreatedAt { get; init; }
}