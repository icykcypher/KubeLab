using Microsoft.AspNetCore.Authorization;

namespace OrchestrationService.Services.Authorization;

public class PermissionRequirement(string requiredPermissions) : IAuthorizationRequirement
{
        public string RequiredRoles { get; } =  requiredPermissions;
}