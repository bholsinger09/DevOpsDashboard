using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Hubs;
using DevOpsDashboard.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Octokit;

namespace DevOpsDashboard.API.Services;

public class GitHubService : IGitHubService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly IConfiguration _configuration;
    private readonly GitHubClient _gitHubClient;

    public GitHubService(
        ApplicationDbContext context,
        IHubContext<DashboardHub> hubContext,
        IConfiguration configuration)
    {
        _context = context;
        _hubContext = hubContext;
        _configuration = configuration;

        var token = _configuration["GitHub:Token"];
        _gitHubClient = new GitHubClient(new ProductHeaderValue("DevOpsDashboard"));
        
        if (!string.IsNullOrEmpty(token) && token != "YOUR_GITHUB_TOKEN_HERE")
        {
            _gitHubClient.Credentials = new Credentials(token);
        }
    }

    public async Task SyncGitHubDataAsync()
    {
        var owner = _configuration["GitHub:Owner"] ?? "bholsinger09";
        var repo = _configuration["GitHub:Repository"] ?? "DevOpsDashboard";

        try
        {
            await SyncIssuesAsync(owner, repo);
            await SyncPullRequestsAsync(owner, repo);

            await _hubContext.Clients.All.SendAsync("ReceiveGitHubUpdate", new
            {
                Message = "GitHub data synchronized successfully",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing GitHub data: {ex.Message}");
        }
    }

    private async Task SyncIssuesAsync(string owner, string repo)
    {
        try
        {
            var issues = await _gitHubClient.Issue.GetAllForRepository(owner, repo);

            foreach (var issue in issues)
            {
                if (issue.PullRequest != null) continue; // Skip PRs

                var existingIssue = await _context.GitHubIssues
                    .FirstOrDefaultAsync(i => i.GitHubId == issue.Number);

                if (existingIssue == null)
                {
                    var newIssue = new GitHubIssue
                    {
                        GitHubId = issue.Number,
                        Title = issue.Title,
                        Description = issue.Body,
                        State = issue.State.StringValue,
                        Author = issue.User.Login,
                        AssignedTo = issue.Assignee?.Login,
                        Url = issue.HtmlUrl,
                        CreatedAt = issue.CreatedAt.UtcDateTime,
                        UpdatedAt = issue.UpdatedAt?.UtcDateTime,
                        ClosedAt = issue.ClosedAt?.UtcDateTime,
                        CommentsCount = issue.Comments,
                        Labels = string.Join(", ", issue.Labels.Select(l => l.Name))
                    };

                    _context.GitHubIssues.Add(newIssue);
                }
                else
                {
                    existingIssue.State = issue.State.StringValue;
                    existingIssue.UpdatedAt = issue.UpdatedAt?.UtcDateTime;
                    existingIssue.ClosedAt = issue.ClosedAt?.UtcDateTime;
                    existingIssue.CommentsCount = issue.Comments;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing issues: {ex.Message}");
        }
    }

    private async Task SyncPullRequestsAsync(string owner, string repo)
    {
        try
        {
            var prs = await _gitHubClient.PullRequest.GetAllForRepository(owner, repo);

            foreach (var pr in prs)
            {
                var existingPR = await _context.GitHubPullRequests
                    .FirstOrDefaultAsync(p => p.GitHubId == pr.Number);

                if (existingPR == null)
                {
                    var newPR = new GitHubPullRequest
                    {
                        GitHubId = pr.Number,
                        Title = pr.Title,
                        Description = pr.Body,
                        State = pr.State.StringValue,
                        Author = pr.User.Login,
                        Url = pr.HtmlUrl,
                        CreatedAt = pr.CreatedAt.UtcDateTime,
                        UpdatedAt = pr.UpdatedAt.UtcDateTime,
                        MergedAt = pr.MergedAt?.UtcDateTime,
                        ClosedAt = pr.ClosedAt?.UtcDateTime,
                        IsMerged = pr.Merged,
                        SourceBranch = pr.Head.Ref,
                        TargetBranch = pr.Base.Ref,
                        CommentsCount = pr.Comments
                    };

                    _context.GitHubPullRequests.Add(newPR);
                }
                else
                {
                    existingPR.State = pr.State.StringValue;
                    existingPR.UpdatedAt = pr.UpdatedAt.UtcDateTime;
                    existingPR.MergedAt = pr.MergedAt?.UtcDateTime;
                    existingPR.ClosedAt = pr.ClosedAt?.UtcDateTime;
                    existingPR.IsMerged = pr.Merged;
                    existingPR.CommentsCount = pr.Comments;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing pull requests: {ex.Message}");
        }
    }
}
