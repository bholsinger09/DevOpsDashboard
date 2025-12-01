namespace DevOpsDashboard.API.Models;

public class GitHubIssue
{
    public int Id { get; set; }
    public long GitHubId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string State { get; set; }
    public required string Author { get; set; }
    public string? AssignedTo { get; set; }
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int CommentsCount { get; set; }
    public string? Labels { get; set; }
}
