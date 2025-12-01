using DevOpsDashboard.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsDashboard.Tests.Controllers;

public class DeploymentsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeploymentsController _controller;

    public DeploymentsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new DeploymentsController(_context);
    }

    [Fact]
    public async Task GetDeployments_ReturnsEmptyList_WhenNoDeploymentsExist()
    {
        // Act
        var result = await _controller.GetDeployments();

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeployments_ReturnsLimitedDeployments_WhenLimitSpecified()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            _context.Deployments.Add(new Deployment
            {
                Environment = $"Env{i}",
                Version = $"v1.{i}",
                DeployedBy = "Test User"
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDeployments(limit: 5);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateDeployment_SetsStartedAtToNow()
    {
        // Arrange
        var deployment = new Deployment
        {
            Environment = "Production",
            Version = "v1.0.0",
            DeployedBy = "Admin"
        };

        // Act
        var result = await _controller.CreateDeployment(deployment);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdDeployment = (result.Result as CreatedAtActionResult)!.Value as Deployment;
        
        createdDeployment!.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateDeployment_SetsCompletedAtAndDuration_WhenStatusIsSuccess()
    {
        // Arrange
        var deployment = new Deployment
        {
            Environment = "Test",
            Version = "v1.0.0",
            DeployedBy = "User",
            Status = DeploymentStatus.InProgress,
            StartedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        _context.Deployments.Add(deployment);
        await _context.SaveChangesAsync();

        deployment.Status = DeploymentStatus.Success;

        // Act
        await _controller.UpdateDeployment(deployment.Id, deployment);

        // Assert
        var updated = await _context.Deployments.FindAsync(deployment.Id);
        updated!.CompletedAt.Should().NotBeNull();
        updated.DurationSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetStats_ReturnsCorrectStatistics()
    {
        // Arrange
        var deployments = new[]
        {
            new Deployment { Environment = "Prod", Version = "v1", DeployedBy = "User1", Status = DeploymentStatus.Success },
            new Deployment { Environment = "Prod", Version = "v2", DeployedBy = "User1", Status = DeploymentStatus.Success },
            new Deployment { Environment = "Test", Version = "v1", DeployedBy = "User2", Status = DeploymentStatus.Failed },
            new Deployment { Environment = "Dev", Version = "v1", DeployedBy = "User3", Status = DeploymentStatus.InProgress }
        };
        _context.Deployments.AddRange(deployments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetStats();

        // Assert
        var stats = result.Value;
        var successfulProp = stats!.GetType().GetProperty("Successful");
        var failedProp = stats.GetType().GetProperty("Failed");
        var totalProp = stats.GetType().GetProperty("Total");

        successfulProp!.GetValue(stats).Should().Be(2);
        failedProp!.GetValue(stats).Should().Be(1);
        totalProp!.GetValue(stats).Should().Be(4);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
