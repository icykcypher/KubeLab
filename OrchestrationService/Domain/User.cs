using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrchestrationService.Domain;

public class User
{
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("PublicId")]
        public string PublicId { get; set; } = string.Empty;
        [BsonElement("Class")]
        public string Class { get; set; } = string.Empty;
        [BsonElement("Name")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("Password")]
        public string PasswordHash { get; set; } = string.Empty;
        [BsonElement("Role")]
        public Role Role { get; set; }
}

public enum Role
{
        Admin = 1,
        Student = 2
}