using MedicalPrintServer.Application.DTOs.Printer;

namespace MedicalPrintServer.Application.Contracts.Services;

public interface IPrinterService
{
    Task<List<PrinterResponse>> GetAllAsync();

    Task<PrinterResponse?> GetByIdAsync(Guid id);

    Task<PrinterResponse> CreateAsync(CreatePrinterRequest request);

    Task<bool> UpdateAsync(Guid id, UpdatePrinterRequest request);

    Task<bool> DeleteAsync(Guid id);
}