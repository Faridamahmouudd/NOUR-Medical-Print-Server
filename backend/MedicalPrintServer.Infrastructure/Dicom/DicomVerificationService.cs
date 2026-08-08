using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MedicalPrintServer.Infrastructure.Dicom;

public class DicomVerificationService :
    DicomService,
    IDicomServiceProvider,
    IDicomCEchoProvider,
    IDicomCStoreProvider
{
    private readonly ILogger<DicomVerificationService> _logger;

    public DicomVerificationService(
        INetworkStream stream,
        Encoding fallbackEncoding,
        ILogger log,
        DicomServiceDependencies dependencies,
        ILogger<DicomVerificationService> logger)
        : base(stream, fallbackEncoding, log, dependencies)
    {
        _logger = logger;
    }

    public Task OnReceiveAssociationRequestAsync(
        DicomAssociation association)
    {
        if (!string.Equals(
                association.CalledAE,
                "NOUR_PRINT",
                StringComparison.OrdinalIgnoreCase))
        {
            return SendAssociationRejectAsync(
                DicomRejectResult.Permanent,
                DicomRejectSource.ServiceUser,
                DicomRejectReason.CalledAENotRecognized);
        }

        foreach (var presentationContext in association.PresentationContexts)
        {
            // C-ECHO
            if (presentationContext.AbstractSyntax == DicomUID.Verification)
            {
                presentationContext.AcceptTransferSyntaxes(
                    DicomTransferSyntax.ImplicitVRLittleEndian,
                    DicomTransferSyntax.ExplicitVRLittleEndian);
            }

            // C-STORE - accept storage SOP classes
            if (presentationContext.AbstractSyntax.StorageCategory !=
                DicomStorageCategory.None)
            {
                presentationContext.AcceptTransferSyntaxes(
                    DicomTransferSyntax.ImplicitVRLittleEndian,
                    DicomTransferSyntax.ExplicitVRLittleEndian,
                    DicomTransferSyntax.ExplicitVRBigEndian);
            }
        }

        return SendAssociationAcceptAsync(association);
    }

    public Task OnReceiveAssociationReleaseRequestAsync()
    {
        return SendAssociationReleaseResponseAsync();
    }

    public void OnReceiveAbort(
        DicomAbortSource source,
        DicomAbortReason reason)
    {
        _logger.LogWarning(
            "DICOM association aborted. Source: {Source}, Reason: {Reason}",
            source,
            reason);
    }

    public void OnConnectionClosed(Exception? exception)
    {
        if (exception is null)
        {
            _logger.LogInformation("DICOM connection closed.");
        }
        else
        {
            _logger.LogError(
                exception,
                "DICOM connection closed with error.");
        }
    }

    public Task<DicomCEchoResponse> OnCEchoRequestAsync(
        DicomCEchoRequest request)
    {
        _logger.LogInformation("DICOM C-ECHO received.");

        return Task.FromResult(
            new DicomCEchoResponse(
                request,
                DicomStatus.Success));
    }

    public async Task<DicomCStoreResponse> OnCStoreRequestAsync(
        DicomCStoreRequest request)
    {
        try
        {
            var receivedFolder = Path.Combine(
                AppContext.BaseDirectory,
                "ReceivedDicom");

            Directory.CreateDirectory(receivedFolder);

            var sopInstanceUid =
                request.File.Dataset.GetSingleValueOrDefault(
                    DicomTag.SOPInstanceUID,
                    Guid.NewGuid().ToString());

            var safeFileName = sopInstanceUid.Replace(".", "_");

            var filePath = Path.Combine(
                receivedFolder,
                $"{safeFileName}.dcm");

            await request.File.SaveAsync(filePath);

            _logger.LogInformation(
                "DICOM file received and saved: {FilePath}",
                filePath);

            return new DicomCStoreResponse(
                request,
                DicomStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to save received DICOM file.");

            return new DicomCStoreResponse(
                request,
                DicomStatus.ProcessingFailure);
        }
    }

    public Task OnCStoreRequestExceptionAsync(
        string tempFileName,
        Exception e)
    {
        _logger.LogError(
            e,
            "C-STORE request error. Temp file: {TempFileName}",
            tempFileName);

        return Task.CompletedTask;
    }
}