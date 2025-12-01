namespace DevOpsDashboard.API.Services;

public interface IDeploymentService
{
    Task NotifyDeploymentStatusAsync(int deploymentId);
}
