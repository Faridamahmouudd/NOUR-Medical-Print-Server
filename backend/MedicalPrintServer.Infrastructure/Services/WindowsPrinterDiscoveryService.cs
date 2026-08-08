using System.Drawing.Printing;

namespace MedicalPrintServer.Infrastructure.Services;

public class WindowsPrinterDiscoveryService
{
    public List<string> GetInstalledPrinters()
    {
        var printers = new List<string>();

        foreach (string printerName in PrinterSettings.InstalledPrinters)
        {
            printers.Add(printerName);
        }

        return printers;
    }
}