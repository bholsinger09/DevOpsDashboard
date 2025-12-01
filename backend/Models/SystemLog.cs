namespace DevOpsDashboard.API.Models;

public class SystemLog
{
    public int Id { get; set; }
    public LogLevel Level { get; set; }
    public required string Source { get; set; }
    public required string Message { get; set; }
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? AdditionalData { get; set; }
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}
