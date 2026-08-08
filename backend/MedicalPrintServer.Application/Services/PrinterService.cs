using MedicalPrintServer.Application.Contracts.Repositories;
using MedicalPrintServer.Application.Contracts.Services;
using MedicalPrintServer.Application.DTOs.Printer;
using MedicalPrintServer.Domain.Entities;

namespace MedicalPrintServer.Application.Services;

public sealed class PrinterService : IPrinterService
{
    private readonly IRepository<Printer> _repository;

    public PrinterService(IRepository<Printer> repository)
    {
        _repository = repository;
    }

    public async Task<List<PrinterResponse>> GetAllAsync()
    {
        var printers = await _repository.GetAllAsync();

        return printers.Select(printer => new PrinterResponse
        {
            Id = printer.Id,
            Name = printer.Name,
            WindowsPrinterName = printer.WindowsPrinterName,
            IpAddress = printer.IpAddress,
            AeTitle = printer.AeTitle,
            DefaultPaperSize = printer.DefaultPaperSize,
            IsDefault = printer.IsDefault,
            IsActive = printer.IsActive,
            CreatedAt = printer.CreatedAt
        }).ToList();
    }

    public async Task<PrinterResponse?> GetByIdAsync(Guid id)
    {
        var printer = await _repository.GetByIdAsync(id);

        if (printer is null)
            return null;

        return new PrinterResponse
        {
            Id = printer.Id,
            Name = printer.Name,
            WindowsPrinterName = printer.WindowsPrinterName,
            IpAddress = printer.IpAddress,
            AeTitle = printer.AeTitle,
            DefaultPaperSize = printer.DefaultPaperSize,
            IsDefault = printer.IsDefault,
            IsActive = printer.IsActive,
            CreatedAt = printer.CreatedAt
        };
    }

    public async Task<PrinterResponse> CreateAsync(
        CreatePrinterRequest request)
    {
        var printer = new Printer
        {
            Name = request.Name,
            WindowsPrinterName = request.WindowsPrinterName,
            IpAddress = request.IpAddress,
            AeTitle = request.AeTitle,
            DefaultPaperSize = request.DefaultPaperSize,
            IsDefault = request.IsDefault
        };

        await _repository.AddAsync(printer);
        await _repository.SaveChangesAsync();

        return new PrinterResponse
        {
            Id = printer.Id,
            Name = printer.Name,
            WindowsPrinterName = printer.WindowsPrinterName,
            IpAddress = printer.IpAddress,
            AeTitle = printer.AeTitle,
            DefaultPaperSize = printer.DefaultPaperSize,
            IsDefault = printer.IsDefault,
            IsActive = printer.IsActive,
            CreatedAt = printer.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdatePrinterRequest request)
    {
        var printer = await _repository.GetByIdAsync(id);

        if (printer is null)
            return false;

        printer.Name = request.Name;
        printer.WindowsPrinterName = request.WindowsPrinterName;
        printer.IpAddress = request.IpAddress;
        printer.AeTitle = request.AeTitle;
        printer.DefaultPaperSize = request.DefaultPaperSize;
        printer.IsDefault = request.IsDefault;
        printer.IsActive = request.IsActive;
        printer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(printer);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var printer = await _repository.GetByIdAsync(id);

        if (printer is null)
            return false;

        await _repository.DeleteAsync(printer);
        await _repository.SaveChangesAsync();

        return true;
    }
}