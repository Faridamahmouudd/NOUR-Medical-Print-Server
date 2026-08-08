using FellowOakDicom.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicalPrintServer.Infrastructure.Dicom;

public class DicomServerHostedService : BackgroundService
{
    private readonly ILogger<DicomServerHostedService> _logger;
    private IDicomServer? _server;

    public DicomServerHostedService(
        ILogger<DicomServerHostedService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            _server = DicomServerFactory.Create<DicomVerificationService>(11112);

            _logger.LogInformation(
                "NOUR DICOM Server started - AE Title: NOUR_PRINT - Port: 11112");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start DICOM Server.");
        }
    }

    public override void Dispose()
    {
        _server?.Dispose();
        base.Dispose();
    }
}