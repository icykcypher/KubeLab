using MongoDB.Bson;
using MongoDB.Driver;
using OrchestrationService.Domain;
using OrchestrationService.Model.Requests;

namespace OrchestrationService.Repositories;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
        private readonly IMongoCollection<User> _collection = database.GetCollection<User>("User");

        public async Task<User> GetUserByUsername(string username)
                => await _collection.Find(x => x.Username == username).FirstOrDefaultAsync();

        public async Task<bool> UserExists(string username)
                => await _collection.CountDocumentsAsync(x => x.Username == username) > 0;

        public async Task<bool> UserExists(ObjectId id)
                => await _collection.Find(x => x.Id == id).AnyAsync();

        public async Task AddUser(User user) => await _collection.InsertOneAsync(user);

        public async Task UpdateUser(User user) => await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);
        
        public async Task DeleteUser(ObjectId id) => await _collection.DeleteOneAsync(x => x.Id == id);
        public async Task<bool> UserExistsByPublicId(string id)
                => await _collection.Find(x => x.PublicId == id).AnyAsync();

        public async Task<User?> GetUserByPublicId(string studentPublicId) 
                => await _collection.Find(x => x.PublicId == studentPublicId).FirstOrDefaultAsync();
}