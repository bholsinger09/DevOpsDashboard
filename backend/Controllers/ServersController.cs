using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Server>>> GetServers()
    {
        return await _context.Servers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Server>> GetServer(int id)
    {
        var server = await _context.Servers.FindAsync(id);

        if (server == null)
        {
            return NotFound();
        }

        return server;
    }

    [HttpPost]
    public async Task<ActionResult<Server>> CreateServer(Server server)
    {
        server.CreatedAt = DateTime.UtcNow;
        server.UpdatedAt = DateTime.UtcNow;
        
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetServer), new { id = server.Id }, server);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateServer(int id, Server server)
    {
        if (id != server.Id)
        {
            return BadRequest();
        }

        server.UpdatedAt = DateTime.UtcNow;
        _context.Entry(server).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServerExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServer(int id)
    {
        var server = await _context.Servers.FindAsync(id);
        if (server == null)
        {
            return NotFound();
        }

        _context.Servers.Remove(server);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var total = await _context.Servers.CountAsync();
        var online = await _context.Servers.CountAsync(s => s.Status == ServerStatus.Online);
        var offline = await _context.Servers.CountAsync(s => s.Status == ServerStatus.Offline);
        var degraded = await _context.Servers.CountAsync(s => s.Status == ServerStatus.Degraded);

        return new
        {
            Total = total,
            Online = online,
            Offline = offline,
            Degraded = degraded
        };
    }

    private bool ServerExists(int id)
    {
        return _context.Servers.Any(e => e.Id == id);
    }
}
