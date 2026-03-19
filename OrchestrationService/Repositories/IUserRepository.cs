using MongoDB.Bson;
using OrchestrationService.Domain;

namespace OrchestrationService.Repositories;

public interface IUserRepository
{
        Task<User> GetUserByUsername(string username);
        Task<bool> UserExists(string username);
        Task<bool> UserExists(ObjectId id);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(ObjectId id);
        Task<bool> UserExistsByPublicId(string id);
        Task<User?> GetUserByPublicId(string studentPublicId);
}