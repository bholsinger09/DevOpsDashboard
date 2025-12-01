using DevOpsDashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsDashboard.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Server> Servers { get; set; }
    public DbSet<GitHubIssue> GitHubIssues { get; set; }
    public DbSet<GitHubPullRequest> GitHubPullRequests { get; set; }
    public DbSet<Deployment> Deployments { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes for better query performance
        modelBuilder.Entity<Server>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<GitHubIssue>()
            .HasIndex(i => i.GitHubId)
            .IsUnique();

        modelBuilder.Entity<GitHubPullRequest>()
            .HasIndex(pr => pr.GitHubId)
            .IsUnique();

        modelBuilder.Entity<Deployment>()
            .HasIndex(d => d.Status);

        modelBuilder.Entity<SystemLog>()
            .HasIndex(l => l.Timestamp);
    }
}
