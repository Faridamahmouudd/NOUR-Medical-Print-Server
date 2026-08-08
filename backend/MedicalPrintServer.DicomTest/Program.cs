using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;

var rows = 256;
var columns = 256;

var dataset = new DicomDataset(
    DicomTransferSyntax.ExplicitVRLittleEndian
);

dataset.AddOrUpdate(
    DicomTag.SOPClassUID,
    DicomUID.SecondaryCaptureImageStorage
);

dataset.AddOrUpdate(
    DicomTag.SOPInstanceUID,
    DicomUIDGenerator.GenerateDerivedFromUUID().UID
);

dataset.AddOrUpdate(
    DicomTag.StudyInstanceUID,
    DicomUIDGenerator.GenerateDerivedFromUUID().UID
);

dataset.AddOrUpdate(
    DicomTag.SeriesInstanceUID,
    DicomUIDGenerator.GenerateDerivedFromUUID().UID
);

dataset.AddOrUpdate(DicomTag.PatientName, "TEST^PATIENT");
dataset.AddOrUpdate(DicomTag.PatientID, "TEST001");
dataset.AddOrUpdate(DicomTag.PatientSex, "O");

dataset.AddOrUpdate(DicomTag.Modality, "OT");
dataset.AddOrUpdate(DicomTag.StudyDescription, "NOUR DICOM TEST");
dataset.AddOrUpdate(DicomTag.SeriesDescription, "Preview Test");
dataset.AddOrUpdate(DicomTag.InstanceNumber, "1");

// Image structure must exist before DicomPixelData.Create
dataset.AddOrUpdate(DicomTag.Rows, (ushort)rows);
dataset.AddOrUpdate(DicomTag.Columns, (ushort)columns);
dataset.AddOrUpdate(DicomTag.BitsAllocated, (ushort)8);
dataset.AddOrUpdate(
    DicomTag.PhotometricInterpretation,
    PhotometricInterpretation.Monochrome2.Value
);

var pixelData = DicomPixelData.Create(
    dataset,
    true
);

pixelData.SamplesPerPixel = 1;
pixelData.BitsStored = 8;
pixelData.HighBit = 7;
pixelData.PixelRepresentation = PixelRepresentation.Unsigned;

var pixels = new byte[rows * columns];

for (var y = 0; y < rows; y++)
{
    for (var x = 0; x < columns; x++)
    {
        pixels[(y * columns) + x] =
            (byte)((x + y) / 2);
    }
}

pixelData.AddFrame(
    new MemoryByteBuffer(pixels)
);

var file = new DicomFile(dataset);

file.FileMetaInfo.TransferSyntax =
    DicomTransferSyntax.ExplicitVRLittleEndian;

var outputPath = Path.Combine(
    AppContext.BaseDirectory,
    "sample2.dcm"
);

await file.SaveAsync(outputPath);

Console.WriteLine("DICOM sample created:");
Console.WriteLine(outputPath);