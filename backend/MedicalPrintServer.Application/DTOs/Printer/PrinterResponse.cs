namespace MedicalPrintServer.Application.DTOs.Printer;
public sealed class PrinterResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string WindowsPrinterName { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public string? AeTitle { get; set; }

    public string DefaultPaperSize { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}