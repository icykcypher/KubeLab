using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrchestrationService.Model.Infrastructure;

namespace OrchestrationService.Model.Domain;

public sealed class Route
{
        [BsonId]
        public ObjectId Id { get; init; }
        public ObjectId NamespaceId { get; init; }
        public ObjectId ServiceId { get; init; }

        public string Host { get; init; } = default!;
        public string Path { get; init; } = "/";
        public RoutingStrategy Strategy { get; init; }
        public int ActiveRevision { get; set; }
}