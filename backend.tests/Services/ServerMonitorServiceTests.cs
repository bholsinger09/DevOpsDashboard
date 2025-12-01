using DevOpsDashboard.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevOpsDashboard.Tests.Services;

public class ServerMonitorServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IHubContext<DashboardHub>> _mockHubContext;
    private readonly ServerMonitorService _service;

    public ServerMonitorServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockHubContext = new Mock<IHubContext<DashboardHub>>();

        // Setup mock for SignalR
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
        mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);

        _service = new ServerMonitorService(_context, _mockHubContext.Object);
    }

    [Fact]
    public async Task CheckServerAsync_UpdatesServerStatus_WhenServerExists()
    {
        // Arrange
        var server = new Server
        {
            Name = "Test Server",
            Url = "https://www.google.com",
            Status = ServerStatus.Unknown
        };
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();

        // Act
        await _service.CheckServerAsync(server.Id);

        // Assert
        var updatedServer = await _context.Servers.FindAsync(server.Id);
        updatedServer.Should().NotBeNull();
        updatedServer!.LastChecked.Should().NotBeNull();
        updatedServer.LastChecked.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task CheckServerAsync_DoesNotThrow_WhenServerDoesNotExist()
    {
        // Act & Assert
        var act = async () => await _service.CheckServerAsync(999);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CheckAllServersAsync_ChecksAllServers_WhenMultipleServersExist()
    {
        // Arrange
        var servers = new[]
        {
            new Server { Name = "Server 1", Url = "https://www.google.com" },
            new Server { Name = "Server 2", Url = "https://www.microsoft.com" }
        };
        _context.Servers.AddRange(servers);
        await _context.SaveChangesAsync();

        // Act
        await _service.CheckAllServersAsync();

        // Assert
        var allServers = await _context.Servers.ToListAsync();
        allServers.Should().HaveCount(2);
        allServers.Should().OnlyContain(s => s.LastChecked != null);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
