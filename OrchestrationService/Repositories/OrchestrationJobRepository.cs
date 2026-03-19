using MongoDB.Driver;
using OrchestrationService.Domain;

namespace OrchestrationService.Repositories;

public class OrchestrationJobRepository(IMongoDatabase db) : IOrchestrationJobRepository
{
        private readonly IMongoCollection<OrchestrationJob> _jobs = db.GetCollection<OrchestrationJob>("OrchestrationJobs");

        public async Task AddJobAsync(OrchestrationJob job) =>
                await _jobs.InsertOneAsync(job);

        public async Task<OrchestrationJob?> GetJobByIdAsync(MongoDB.Bson.ObjectId id) =>
                await _jobs.Find(j => j.Id == id).FirstOrDefaultAsync();
}