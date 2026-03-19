using Microsoft.AspNetCore.Authorization;

namespace OrchestrationService.Services.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
        protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                PermissionRequirement requirement)
        {
                if (context.User.Identity?.IsAuthenticated != true)
                        return Task.CompletedTask;

                var userRole = context.User.FindFirst("role")?.Value;

                if (userRole is null)
                        return Task.CompletedTask;

                if (requirement.RequiredRoles.Contains(userRole))
                {
                        context.Succeed(requirement);
                }

                return Task.CompletedTask;
        }
}