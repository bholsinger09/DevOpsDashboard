namespace DevOpsDashboard.API.Models;

public class Deployment
{
    public int Id { get; set; }
    public required string Environment { get; set; }
    public required string Version { get; set; }
    public DeploymentStatus Status { get; set; } = DeploymentStatus.Pending;
    public required string DeployedBy { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public string? ErrorMessage { get; set; }
    public int? DurationSeconds { get; set; }
}

public enum DeploymentStatus
{
    Pending,
    InProgress,
    Success,
    Failed,
    Cancelled
}
