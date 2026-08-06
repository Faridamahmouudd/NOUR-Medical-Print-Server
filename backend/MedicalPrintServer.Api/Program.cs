using MedicalPrintServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection was not found."
    );

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new
{
    applicationName = "NOUR DICOM Print Server",
    version = "1.0.0",
    status = "Running",
    dicomStatus = "Offline",
    dicomAeTitle = "NOUR_PRINT",
    dicomPort = 11112,
    printersCount = 0,
    waitingJobs = 0,
    completedJobs = 0,
    failedJobs = 0,
    serverTime = DateTime.Now
}))
.WithName("GetServerStatus")
.WithOpenApi();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    message = "Medical Print Server API is working",
    checkedAt = DateTime.Now
}))
.WithName("HealthCheck")
.WithOpenApi();

app.Run();