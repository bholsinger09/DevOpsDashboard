using System.Diagnostics;
using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Hubs;
using DevOpsDashboard.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Services;

public class ServerMonitorService : IServerMonitorService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly HttpClient _httpClient;

    public ServerMonitorService(
        ApplicationDbContext context, 
        IHubContext<DashboardHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task CheckAllServersAsync()
    {
        var servers = await _context.Servers.ToListAsync();

        foreach (var server in servers)
        {
            await CheckServerAsync(server.Id);
        }
    }

    public async Task CheckServerAsync(int serverId)
    {
        var server = await _context.Servers.FindAsync(serverId);
        if (server == null) return;

        var stopwatch = Stopwatch.StartNew();
        ServerStatus previousStatus = server.Status;

        try
        {
            var response = await _httpClient.GetAsync(server.Url);
            stopwatch.Stop();

            server.Status = response.IsSuccessStatusCode ? ServerStatus.Online : ServerStatus.Degraded;
            server.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
            server.LastChecked = DateTime.UtcNow;

            // Calculate uptime percentage (simplified)
            if (server.Status == ServerStatus.Online)
            {
                server.UptimePercentage = Math.Min(100, server.UptimePercentage + 1);
            }
            else
            {
                server.UptimePercentage = Math.Max(0, server.UptimePercentage - 5);
            }
        }
        catch (Exception)
        {
            stopwatch.Stop();
            server.Status = ServerStatus.Offline;
            server.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
            server.LastChecked = DateTime.UtcNow;
            server.UptimePercentage = Math.Max(0, server.UptimePercentage - 10);
        }

        server.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Send real-time update if status changed
        if (previousStatus != server.Status)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveServerStatusUpdate", new
            {
                server.Id,
                server.Name,
                server.Status,
                server.ResponseTimeMs,
                server.UptimePercentage,
                server.LastChecked
            });
        }
    }
}
