using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemLog>>> GetLogs(
        [FromQuery] LogLevel? level = null,
        [FromQuery] string? source = null,
        [FromQuery] int? limit = 100)
    {
        var query = _context.SystemLogs.AsQueryable();

        if (level.HasValue)
        {
            query = query.Where(l => l.Level == level.Value);
        }

        if (!string.IsNullOrEmpty(source))
        {
            query = query.Where(l => l.Source.Contains(source));
        }

        return await query
            .OrderByDescending(l => l.Timestamp)
            .Take(limit.Value)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SystemLog>> GetLog(int id)
    {
        var log = await _context.SystemLogs.FindAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        return log;
    }

    [HttpPost]
    public async Task<ActionResult<SystemLog>> CreateLog(SystemLog log)
    {
        log.Timestamp = DateTime.UtcNow;
        
        _context.SystemLogs.Add(log);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLog), new { id = log.Id }, log);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(int id)
    {
        var log = await _context.SystemLogs.FindAsync(id);
        if (log == null)
        {
            return NotFound();
        }

        _context.SystemLogs.Remove(log);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("cleanup")]
    public async Task<IActionResult> CleanupOldLogs([FromQuery] int daysOld = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        var oldLogs = await _context.SystemLogs
            .Where(l => l.Timestamp < cutoffDate)
            .ToListAsync();

        _context.SystemLogs.RemoveRange(oldLogs);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Deleted {oldLogs.Count} logs older than {daysOld} days" });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var totalLogs = await _context.SystemLogs.CountAsync();
        var errorLogs = await _context.SystemLogs.CountAsync(l => l.Level == LogLevel.Error);
        var warningLogs = await _context.SystemLogs.CountAsync(l => l.Level == LogLevel.Warning);
        var infoLogs = await _context.SystemLogs.CountAsync(l => l.Level == LogLevel.Info);

        return new
        {
            Total = totalLogs,
            Error = errorLogs,
            Warning = warningLogs,
            Info = infoLogs
        };
    }
}
