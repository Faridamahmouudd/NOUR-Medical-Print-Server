using MedicalPrintServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MedicalPrintServer.Application.Contracts.Repositories;
using MedicalPrintServer.Application.Contracts.Services;
using MedicalPrintServer.Application.Services;
using MedicalPrintServer.Infrastructure.Repositories;
using MedicalPrintServer.Infrastructure.Services;
using MedicalPrintServer.Infrastructure.Dicom;
using FellowOakDicom;
using FellowOakDicom.Imaging;

var builder = WebApplication.CreateBuilder(args);
new DicomSetupBuilder()
    .RegisterServices(services =>
        services
            .AddFellowOakDicom()
            .AddImageManager<ImageSharpImageManager>())
    .Build();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection was not found."
    );

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(Repository<>)
);

builder.Services.AddScoped<IPrinterService, PrinterService>();
builder.Services.AddScoped<WindowsPrinterDiscoveryService>();


builder.Services.AddHostedService<DicomServerHostedService>();

builder.Services.AddControllers();

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
    dicomStatus = "Running",
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

app.MapControllers();

app.Run();