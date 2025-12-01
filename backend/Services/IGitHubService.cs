namespace DevOpsDashboard.API.Services;

public interface IGitHubService
{
    Task SyncGitHubDataAsync();
}
