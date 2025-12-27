using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrchestrationService.Model.Domain;

public class User
{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<string> Roles { get; set; } = [];
        public IDictionary<string, string> Apps { get; set; } = [];
}