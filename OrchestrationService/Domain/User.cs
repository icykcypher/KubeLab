using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrchestrationService.Domain;

public class User
{
        [BsonId]
        public ObjectId Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<string> Roles { get; set; } = [];
        public IDictionary<string, string> Apps { get; set; } = [];
}