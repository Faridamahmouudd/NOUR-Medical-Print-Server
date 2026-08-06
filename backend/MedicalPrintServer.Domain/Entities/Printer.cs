namespace MedicalPrintServer.Domain.Entities;

public sealed class Printer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string WindowsPrinterName { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public string? AeTitle { get; set; }

    public string DefaultPaperSize { get; set; } = "A3";

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}