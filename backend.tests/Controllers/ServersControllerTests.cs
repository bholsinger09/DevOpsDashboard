using DevOpsDashboard.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsDashboard.Tests.Controllers;

public class ServersControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ServersController _controller;

    public ServersControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new ServersController(_context);
    }

    [Fact]
    public async Task GetServers_ReturnsEmptyList_WhenNoServersExist()
    {
        // Act
        var result = await _controller.GetServers();

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetServers_ReturnsAllServers_WhenServersExist()
    {
        // Arrange
        var servers = new[]
        {
            new Server { Name = "Server 1", Url = "https://server1.com" },
            new Server { Name = "Server 2", Url = "https://server2.com" }
        };
        _context.Servers.AddRange(servers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetServers();

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetServer_ReturnsNotFound_WhenServerDoesNotExist()
    {
        // Act
        var result = await _controller.GetServer(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetServer_ReturnsServer_WhenServerExists()
    {
        // Arrange
        var server = new Server { Name = "Test Server", Url = "https://test.com" };
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetServer(server.Id);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Test Server");
        result.Value.Url.Should().Be("https://test.com");
    }

    [Fact]
    public async Task CreateServer_AddsServerToDatabase()
    {
        // Arrange
        var server = new Server { Name = "New Server", Url = "https://new.com" };

        // Act
        var result = await _controller.CreateServer(server);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var createdServer = createdResult!.Value as Server;
        
        createdServer.Should().NotBeNull();
        createdServer!.Name.Should().Be("New Server");
        createdServer.Id.Should().BeGreaterThan(0);

        // Verify it's in the database
        var dbServer = await _context.Servers.FindAsync(createdServer.Id);
        dbServer.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateServer_ReturnsNotFound_WhenServerDoesNotExist()
    {
        // Arrange
        var server = new Server { Id = 999, Name = "Test", Url = "https://test.com" };

        // Act
        var result = await _controller.UpdateServer(999, server);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateServer_UpdatesServer_WhenServerExists()
    {
        // Arrange
        var server = new Server { Name = "Original", Url = "https://original.com" };
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();

        server.Name = "Updated";
        server.Url = "https://updated.com";

        // Act
        var result = await _controller.UpdateServer(server.Id, server);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var updatedServer = await _context.Servers.FindAsync(server.Id);
        updatedServer!.Name.Should().Be("Updated");
        updatedServer.Url.Should().Be("https://updated.com");
    }

    [Fact]
    public async Task DeleteServer_ReturnsNotFound_WhenServerDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteServer(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteServer_RemovesServer_WhenServerExists()
    {
        // Arrange
        var server = new Server { Name = "To Delete", Url = "https://delete.com" };
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();
        var serverId = server.Id;

        // Act
        var result = await _controller.DeleteServer(serverId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var deletedServer = await _context.Servers.FindAsync(serverId);
        deletedServer.Should().BeNull();
    }

    [Fact]
    public async Task GetStats_ReturnsCorrectStatistics()
    {
        // Arrange
        var servers = new[]
        {
            new Server { Name = "Online", Url = "https://online.com", Status = ServerStatus.Online },
            new Server { Name = "Offline", Url = "https://offline.com", Status = ServerStatus.Offline },
            new Server { Name = "Degraded", Url = "https://degraded.com", Status = ServerStatus.Degraded }
        };
        _context.Servers.AddRange(servers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetStats();

        // Assert
        result.Value.Should().NotBeNull();
        var stats = result.Value;
        
        var totalProp = stats!.GetType().GetProperty("Total");
        var onlineProp = stats.GetType().GetProperty("Online");
        var offlineProp = stats.GetType().GetProperty("Offline");
        var degradedProp = stats.GetType().GetProperty("Degraded");

        totalProp!.GetValue(stats).Should().Be(3);
        onlineProp!.GetValue(stats).Should().Be(1);
        offlineProp!.GetValue(stats).Should().Be(1);
        degradedProp!.GetValue(stats).Should().Be(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
