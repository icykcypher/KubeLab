namespace OrchestrationService.Model.Dtos;

public class UsersSeedingDto
{
        public Dictionary<string, List<UserSeedingDto>> UsersInClass { get; set; } = [];
}

public class UserSeedingDto
{
        public string Username { get; set; } = string.Empty;
}