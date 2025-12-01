using DevOpsDashboard.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsDashboard.Tests.Controllers;

public class LogsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly LogsController _controller;

    public LogsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new LogsController(_context);
    }

    [Fact]
    public async Task GetLogs_FiltersBy LogLevel_WhenLevelProvided()
    {
        // Arrange
        var logs = new[]
        {
            new SystemLog { Level = LogLevel.Error, Source = "API", Message = "Error message" },
            new SystemLog { Level = LogLevel.Info, Source = "API", Message = "Info message" },
            new SystemLog { Level = LogLevel.Warning, Source = "API", Message = "Warning message" }
        };
        _context.SystemLogs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLogs(level: LogLevel.Error);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public async Task GetLogs_FiltersBySource_WhenSourceProvided()
    {
        // Arrange
        var logs = new[]
        {
            new SystemLog { Level = LogLevel.Info, Source = "API", Message = "API message" },
            new SystemLog { Level = LogLevel.Info, Source = "Database", Message = "DB message" }
        };
        _context.SystemLogs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLogs(source: "API");

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Source.Should().Contain("API");
    }

    [Fact]
    public async Task CreateLog_SetsTimestampToNow()
    {
        // Arrange
        var log = new SystemLog
        {
            Level = LogLevel.Info,
            Source = "Test",
            Message = "Test message"
        };

        // Act
        var result = await _controller.CreateLog(log);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdLog = (result.Result as CreatedAtActionResult)!.Value as SystemLog;
        
        createdLog!.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CleanupOldLogs_RemovesLogsOlderThanSpecifiedDays()
    {
        // Arrange
        var oldLog = new SystemLog
        {
            Level = LogLevel.Info,
            Source = "Test",
            Message = "Old message",
            Timestamp = DateTime.UtcNow.AddDays(-35)
        };
        var recentLog = new SystemLog
        {
            Level = LogLevel.Info,
            Source = "Test",
            Message = "Recent message",
            Timestamp = DateTime.UtcNow.AddDays(-5)
        };
        _context.SystemLogs.AddRange(oldLog, recentLog);
        await _context.SaveChangesAsync();

        // Act
        await _controller.CleanupOldLogs(daysOld: 30);

        // Assert
        var remainingLogs = await _context.SystemLogs.ToListAsync();
        remainingLogs.Should().HaveCount(1);
        remainingLogs.First().Message.Should().Be("Recent message");
    }

    [Fact]
    public async Task GetStats_ReturnsCorrectStatistics()
    {
        // Arrange
        var logs = new[]
        {
            new SystemLog { Level = LogLevel.Error, Source = "API", Message = "Error 1" },
            new SystemLog { Level = LogLevel.Error, Source = "API", Message = "Error 2" },
            new SystemLog { Level = LogLevel.Warning, Source = "API", Message = "Warning" },
            new SystemLog { Level = LogLevel.Info, Source = "API", Message = "Info" }
        };
        _context.SystemLogs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetStats();

        // Assert
        var stats = result.Value;
        var errorProp = stats!.GetType().GetProperty("Error");
        var warningProp = stats.GetType().GetProperty("Warning");
        var infoProp = stats.GetType().GetProperty("Info");

        errorProp!.GetValue(stats).Should().Be(2);
        warningProp!.GetValue(stats).Should().Be(1);
        infoProp!.GetValue(stats).Should().Be(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
