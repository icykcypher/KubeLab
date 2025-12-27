namespace OrchestrationService.Model.Infrastructure;

public enum RevisionStatus
{
        Pending,
        Deploying,
        Active,
        Failed,
        RolledBack
}