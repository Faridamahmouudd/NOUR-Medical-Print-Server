namespace MedicalPrintServer.Application.DTOs.Printer;
public sealed class UpdatePrinterRequest
{
    public string Name { get; set; } = string.Empty;

    public string WindowsPrinterName { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public string? AeTitle { get; set; }

    public string DefaultPaperSize { get; set; } = "A3";

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }
}
