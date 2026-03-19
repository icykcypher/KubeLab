using Microsoft.AspNetCore.Identity;

namespace OrchestrationService.Services.PasswordHashers;

public class PasswordHasher : IPasswordHasher
{
        public string Generate(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool Verify(string password, string hashedPassword) => BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
}