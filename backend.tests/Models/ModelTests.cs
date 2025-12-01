namespace DevOpsDashboard.Tests.Models;

public class ServerTests
{
    [Fact]
    public void Server_ShouldHaveDefaultValues_WhenCreated()
    {
        // Act
        var server = new Server
        {
            Name = "Test Server",
            Url = "https://test.com"
        };

        // Assert
        server.Status.Should().Be(ServerStatus.Unknown);
        server.UptimePercentage.Should().Be(0);
        server.ResponseTimeMs.Should().Be(0);
        server.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        server.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(ServerStatus.Online)]
    [InlineData(ServerStatus.Offline)]
    [InlineData(ServerStatus.Degraded)]
    [InlineData(ServerStatus.Unknown)]
    public void Server_CanSetAllStatusValues(ServerStatus status)
    {
        // Arrange
        var server = new Server
        {
            Name = "Test",
            Url = "https://test.com"
        };

        // Act
        server.Status = status;

        // Assert
        server.Status.Should().Be(status);
    }
}

public class DeploymentTests
{
    [Fact]
    public void Deployment_ShouldHaveDefaultValues_WhenCreated()
    {
        // Act
        var deployment = new Deployment
        {
            Environment = "Production",
            Version = "v1.0.0",
            DeployedBy = "Admin"
        };

        // Assert
        deployment.Status.Should().Be(DeploymentStatus.Pending);
        deployment.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(DeploymentStatus.Pending)]
    [InlineData(DeploymentStatus.InProgress)]
    [InlineData(DeploymentStatus.Success)]
    [InlineData(DeploymentStatus.Failed)]
    [InlineData(DeploymentStatus.Cancelled)]
    public void Deployment_CanSetAllStatusValues(DeploymentStatus status)
    {
        // Arrange
        var deployment = new Deployment
        {
            Environment = "Test",
            Version = "v1.0.0",
            DeployedBy = "User"
        };

        // Act
        deployment.Status = status;

        // Assert
        deployment.Status.Should().Be(status);
    }
}

public class SystemLogTests
{
    [Fact]
    public void SystemLog_ShouldHaveDefaultTimestamp_WhenCreated()
    {
        // Act
        var log = new SystemLog
        {
            Level = LogLevel.Info,
            Source = "Test",
            Message = "Test message"
        };

        // Assert
        log.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void SystemLog_CanSetAllLevelValues(LogLevel level)
    {
        // Arrange
        var log = new SystemLog
        {
            Source = "Test",
            Message = "Test"
        };

        // Act
        log.Level = level;

        // Assert
        log.Level.Should().Be(level);
    }
}
