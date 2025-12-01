namespace DevOpsDashboard.API.Models;

public class Server
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public string? Description { get; set; }
    public ServerStatus Status { get; set; } = ServerStatus.Unknown;
    public DateTime? LastChecked { get; set; }
    public int UptimePercentage { get; set; }
    public int ResponseTimeMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ServerStatus
{
    Unknown,
    Online,
    Offline,
    Degraded
}
