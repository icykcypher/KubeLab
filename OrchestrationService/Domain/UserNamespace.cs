using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Domain;

public sealed class UserNamespace
{
        [BsonId]
        public ObjectId Id { get; init; }
        public string UserId { get; init; } = string.Empty;
        public string ClusterId { get; init; } = string.Empty;
        public string Namespace { get; init; } = string.Empty;
        public NamespaceStatus Status { get; set; }
        public DateTime CreatedAt { get; init; }
}