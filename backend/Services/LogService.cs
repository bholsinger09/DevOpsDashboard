using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevOpsDashboard.API.Services;

public class LogService : ILogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<DashboardHub> _hubContext;

    public LogService(
        ApplicationDbContext context,
        IHubContext<DashboardHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task ProcessLogAsync(int logId)
    {
        var log = await _context.SystemLogs.FindAsync(logId);
        if (log == null) return;

        // Send real-time notification for critical logs
        if (log.Level == Models.LogLevel.Error || log.Level == Models.LogLevel.Critical)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLogNotification", new
            {
                log.Id,
                log.Level,
                log.Source,
                log.Message,
                log.Timestamp
            });
        }
    }
}
