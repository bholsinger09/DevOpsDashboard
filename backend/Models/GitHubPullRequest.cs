namespace DevOpsDashboard.API.Models;

public class GitHubPullRequest
{
    public int Id { get; set; }
    public long GitHubId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string State { get; set; }
    public required string Author { get; set; }
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? MergedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public bool IsMerged { get; set; }
    public string? SourceBranch { get; set; }
    public string? TargetBranch { get; set; }
    public int CommentsCount { get; set; }
}
