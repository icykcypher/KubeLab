using MongoDB.Driver;
using OrchestrationService.Domain;

namespace OrchestrationService.Services.MongoInitializers;

public class MongoInitializer(IMongoDatabase database)
{
        public async Task InitializeAsync()
        {
                var collections = await database.ListCollectionNames().ToListAsync();

                if (!collections.Contains("users"))
                {
                        await database.CreateCollectionAsync("users");
                }

                var userCollection = database.GetCollection<User>("users");

                var indexUsernameKey = Builders<User>.IndexKeys.Ascending(x => x.Username);
                var indexPublicIdKey = Builders<User>.IndexKeys.Ascending(x => x.PublicId);
                
                var indexOptions = new CreateIndexOptions { Unique = true };

                await userCollection.Indexes.CreateOneAsync(
                        new CreateIndexModel<User>(indexUsernameKey, indexOptions)
                );
                
                await userCollection.Indexes.CreateOneAsync(
                        new CreateIndexModel<User>(indexPublicIdKey, indexOptions)
                );
        }
}