using DevOpsDashboard.API.Data;
using DevOpsDashboard.API.Hubs;
using DevOpsDashboard.API.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for Ionic app
builder.Services.AddCors(options =>
{
    options.AddPolicy("IonicApp", policy =>
    {
        policy.WithOrigins("http://localhost:8100", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure SignalR
builder.Services.AddSignalR();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Hangfire with memory storage
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMemoryStorage());

builder.Services.AddHangfireServer();

// Register services
builder.Services.AddScoped<IServerMonitorService, ServerMonitorService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IDeploymentService, DeploymentService>();
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseCors("IonicApp");

app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<DashboardHub>("/dashboardHub");

// Configure Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Schedule recurring jobs
RecurringJob.AddOrUpdate<IServerMonitorService>(
    "check-server-uptime",
    service => service.CheckAllServersAsync(),
    Cron.Minutely);

RecurringJob.AddOrUpdate<IGitHubService>(
    "sync-github-data",
    service => service.SyncGitHubDataAsync(),
    "*/5 * * * *");

app.Run();
