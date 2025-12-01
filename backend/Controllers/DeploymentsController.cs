using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeploymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DeploymentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Deployment>>> GetDeployments([FromQuery] int? limit = 50)
    {
        return await _context.Deployments
            .OrderByDescending(d => d.StartedAt)
            .Take(limit.Value)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Deployment>> GetDeployment(int id)
    {
        var deployment = await _context.Deployments.FindAsync(id);

        if (deployment == null)
        {
            return NotFound();
        }

        return deployment;
    }

    [HttpPost]
    public async Task<ActionResult<Deployment>> CreateDeployment(Deployment deployment)
    {
        deployment.StartedAt = DateTime.UtcNow;
        
        _context.Deployments.Add(deployment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeployment), new { id = deployment.Id }, deployment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeployment(int id, Deployment deployment)
    {
        if (id != deployment.Id)
        {
            return BadRequest();
        }

        if (deployment.Status == DeploymentStatus.Success || 
            deployment.Status == DeploymentStatus.Failed || 
            deployment.Status == DeploymentStatus.Cancelled)
        {
            deployment.CompletedAt = DateTime.UtcNow;
            
            if (deployment.CompletedAt.HasValue)
            {
                deployment.DurationSeconds = (int)(deployment.CompletedAt.Value - deployment.StartedAt).TotalSeconds;
            }
        }

        _context.Entry(deployment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DeploymentExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var totalDeployments = await _context.Deployments.CountAsync();
        var successfulDeployments = await _context.Deployments.CountAsync(d => d.Status == DeploymentStatus.Success);
        var failedDeployments = await _context.Deployments.CountAsync(d => d.Status == DeploymentStatus.Failed);
        var inProgressDeployments = await _context.Deployments.CountAsync(d => d.Status == DeploymentStatus.InProgress);

        var successRate = totalDeployments > 0 ? (double)successfulDeployments / totalDeployments * 100 : 0;

        return new
        {
            Total = totalDeployments,
            Successful = successfulDeployments,
            Failed = failedDeployments,
            InProgress = inProgressDeployments,
            SuccessRate = Math.Round(successRate, 2)
        };
    }

    private bool DeploymentExists(int id)
    {
        return _context.Deployments.Any(e => e.Id == id);
    }
}
