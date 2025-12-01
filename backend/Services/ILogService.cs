namespace DevOpsDashboard.API.Services;

public interface ILogService
{
    Task ProcessLogAsync(int logId);
}
