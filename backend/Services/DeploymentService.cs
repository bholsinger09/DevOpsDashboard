using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevOpsDashboard.API.Services;

public class DeploymentService : IDeploymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<DashboardHub> _hubContext;

    public DeploymentService(
        ApplicationDbContext context,
        IHubContext<DashboardHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task NotifyDeploymentStatusAsync(int deploymentId)
    {
        var deployment = await _context.Deployments.FindAsync(deploymentId);
        if (deployment == null) return;

        await _hubContext.Clients.All.SendAsync("ReceiveDeploymentNotification", new
        {
            deployment.Id,
            deployment.Environment,
            deployment.Version,
            deployment.Status,
            deployment.DeployedBy,
            deployment.StartedAt,
            deployment.CompletedAt,
            deployment.DurationSeconds
        });
    }
}
