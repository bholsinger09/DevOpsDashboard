using Microsoft.AspNetCore.SignalR;

namespace DevOpsDashboard.API.Hubs;

public class DashboardHub : Hub
{
    public async Task SendServerStatusUpdate(object serverData)
    {
        await Clients.All.SendAsync("ReceiveServerStatusUpdate", serverData);
    }

    public async Task SendDeploymentNotification(object deploymentData)
    {
        await Clients.All.SendAsync("ReceiveDeploymentNotification", deploymentData);
    }

    public async Task SendLogNotification(object logData)
    {
        await Clients.All.SendAsync("ReceiveLogNotification", logData);
    }

    public async Task SendGitHubUpdate(object gitHubData)
    {
        await Clients.All.SendAsync("ReceiveGitHubUpdate", gitHubData);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
}
