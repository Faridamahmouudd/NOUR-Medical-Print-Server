using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Png;
namespace MedicalPrintServer.Api.Controllers;

[ApiController]
[Route("api/dicom")]
public class DicomController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public DicomController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetReceivedDicomFiles()
    {
        var folder = Path.Combine(
            _environment.ContentRootPath,
            "bin",
            "Debug",
            "net8.0",
            "ReceivedDicom"
        );

        var runtimeFolder = Path.Combine(
            AppContext.BaseDirectory,
            "ReceivedDicom"
        );

        if (Directory.Exists(runtimeFolder))
            folder = runtimeFolder;

        if (!Directory.Exists(folder))
            return Ok(Array.Empty<object>());

        var result = new List<object>();

        foreach (var filePath in Directory.GetFiles(folder, "*.dcm"))
        {
            try
            {
                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;

                result.Add(new
                {
                    fileName = Path.GetFileName(filePath),

                    patientName =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.PatientName,
                            string.Empty),

                    patientId =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.PatientID,
                            string.Empty),

                    modality =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.Modality,
                            string.Empty),

                    studyDate =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.StudyDate,
                            string.Empty),

                    studyInstanceUid =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.StudyInstanceUID,
                            string.Empty),

                    sopInstanceUid =
                        dataset.GetSingleValueOrDefault(
                            DicomTag.SOPInstanceUID,
                            string.Empty),

                    receivedAt =
                        System.IO.File.GetCreationTime(filePath),

                    fileSize =
                        new FileInfo(filePath).Length
                });
            }
            catch (Exception ex)
            {
                result.Add(new
                {
                    fileName = Path.GetFileName(filePath),
                    error = ex.Message
                });
            }
        }

        return Ok(result);
    }

    [HttpGet("{fileName}/preview")]
    public async Task<IActionResult> GetPreview(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);

        if (!safeFileName.EndsWith(
                ".dcm",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid DICOM file name.");
        }

        var folder = Path.Combine(
            AppContext.BaseDirectory,
            "ReceivedDicom"
        );

        var filePath = Path.Combine(
            folder,
            safeFileName
        );

        if (!System.IO.File.Exists(filePath))
            return NotFound("DICOM file not found.");

        try
        {
            var dicomFile =
                await DicomFile.OpenAsync(filePath);

            var pixelData =
                DicomPixelData.Create(
                    dicomFile.Dataset
                );

            if (pixelData.NumberOfFrames == 0)
            {
                return BadRequest(
                    "DICOM file contains no image frames."
                );
            }

            var image =
                new DicomImage(
                    dicomFile.Dataset
                );

            var renderedImage =
                image.RenderImage(0);

            using var imageSharp =
                renderedImage.AsSharpImage();

            using var stream =
                new MemoryStream();

            await imageSharp.SaveAsync(
                stream,
                new PngEncoder()
            );

            return File(
                stream.ToArray(),
                "image/png",
                Path.GetFileNameWithoutExtension(
                    safeFileName
                ) + ".png"
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message =
                    "Could not render DICOM preview.",

                error = ex.Message
            });
        }
    }
}