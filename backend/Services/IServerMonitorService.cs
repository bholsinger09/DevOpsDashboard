namespace DevOpsDashboard.API.Services;

public interface IServerMonitorService
{
    Task CheckAllServersAsync();
    Task CheckServerAsync(int serverId);
}
