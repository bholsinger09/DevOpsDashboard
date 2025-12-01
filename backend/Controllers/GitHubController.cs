using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Models;
using DevOpsDashboard.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IGitHubService _gitHubService;

    public GitHubController(ApplicationDbContext context, IGitHubService gitHubService)
    {
        _context = context;
        _gitHubService = gitHubService;
    }

    [HttpGet("issues")]
    public async Task<ActionResult<IEnumerable<GitHubIssue>>> GetIssues([FromQuery] string? state = null)
    {
        var query = _context.GitHubIssues.AsQueryable();

        if (!string.IsNullOrEmpty(state))
        {
            query = query.Where(i => i.State == state);
        }

        return await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
    }

    [HttpGet("pullrequests")]
    public async Task<ActionResult<IEnumerable<GitHubPullRequest>>> GetPullRequests([FromQuery] string? state = null)
    {
        var query = _context.GitHubPullRequests.AsQueryable();

        if (!string.IsNullOrEmpty(state))
        {
            query = query.Where(pr => pr.State == state);
        }

        return await query.OrderByDescending(pr => pr.CreatedAt).ToListAsync();
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncGitHubData()
    {
        try
        {
            await _gitHubService.SyncGitHubDataAsync();
            return Ok(new { message = "GitHub data synchronized successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to sync GitHub data", error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var openIssues = await _context.GitHubIssues.CountAsync(i => i.State == "open");
        var closedIssues = await _context.GitHubIssues.CountAsync(i => i.State == "closed");
        var openPRs = await _context.GitHubPullRequests.CountAsync(pr => pr.State == "open");
        var mergedPRs = await _context.GitHubPullRequests.CountAsync(pr => pr.IsMerged);

        return new
        {
            Issues = new { Open = openIssues, Closed = closedIssues },
            PullRequests = new { Open = openPRs, Merged = mergedPRs }
        };
    }
}
