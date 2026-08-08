using MedicalPrintServer.Application.Contracts.Services;
using MedicalPrintServer.Application.DTOs.Printer;
using Microsoft.AspNetCore.Mvc;
using MedicalPrintServer.Infrastructure.Services;

namespace MedicalPrintServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PrintersController : ControllerBase
{
private readonly IPrinterService _printerService;
private readonly WindowsPrinterDiscoveryService _windowsPrinterService;

public PrintersController(
    IPrinterService printerService,
    WindowsPrinterDiscoveryService windowsPrinterService)
{
    _printerService = printerService;
    _windowsPrinterService = windowsPrinterService;
}
    [HttpGet]
    public async Task<ActionResult<List<PrinterResponse>>> GetAll()
    {
        var printers = await _printerService.GetAllAsync();
        return Ok(printers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrinterResponse>> GetById(Guid id)
    {
        var printer = await _printerService.GetByIdAsync(id);

        if (printer is null)
            return NotFound();

        return Ok(printer);
    }

    [HttpPost]
    public async Task<ActionResult<PrinterResponse>> Create(
        CreatePrinterRequest request)
    {
        var printer = await _printerService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = printer.Id },
            printer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePrinterRequest request)
    {
        var updated = await _printerService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _printerService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
    [HttpGet("windows")]
public ActionResult<List<string>> GetWindowsPrinters()
{
    var printers = _windowsPrinterService.GetInstalledPrinters();

    return Ok(printers);
}
}