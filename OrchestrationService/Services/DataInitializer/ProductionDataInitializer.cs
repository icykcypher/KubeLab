using MongoDB.Driver;
using OrchestrationService.Domain;
using OrchestrationService.Model.Dtos;
using OrchestrationService.Services.Authorization;
using OrchestrationService.Services.PasswordHashers;

namespace OrchestrationService.Services.DataInitializer;

public sealed class ProductionDataInitializer(
        IMongoDatabase database,
        IPasswordHasher hasher,
        IUserService userService,
        IConfiguration configuration)
{
        public async Task InitializeAsync(List<AdminSeedingDto> admins, UsersSeedingDto users)
        {
                await SeedUsersFromEnvAsync(admins, users);
        }

        private async Task SeedUsersFromEnvAsync(List<AdminSeedingDto> admins, UsersSeedingDto users)
        {
                if (admins.Count <= 0 || users.UsersInClass.Count <= 0)
                        return;

                var db = database.GetCollection<User>("users");
                
                foreach (var u in admins)
                {
                        var exists = await db.Find(x => x.Username == u.Username).AnyAsync();
                        if (exists) continue;

                        var user = new User
                        {
                                Username = u.Username,
                                PasswordHash = hasher.Generate(u.Password),
                                Class = "system",
                                Role = Role.Admin,
                                PublicId = await userService.GenerateUniquePublicId(),
                        };

                        await db.InsertOneAsync(user);
                }
                
                foreach (var c in users.UsersInClass.Keys)
                {
                        foreach (var l in users.UsersInClass.Values)
                        {
                                foreach (var u in l)
                                {
                                        var exists = await db.Find(x => x.Username == u.Username).AnyAsync();
                                        if (exists) continue;

                                        var user = new User
                                        {
                                                Username = u.Username,
                                                PasswordHash = hasher.Generate("jooouda"),
                                                Class = c,
                                                Role = Role.Student,
                                                PublicId = await userService.GenerateUniquePublicId(),
                                        };

                                        await db.InsertOneAsync(user);
                                }
                        }
                }
        }
}