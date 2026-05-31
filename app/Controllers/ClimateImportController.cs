using _10xPV.Models.ClimateImport;
using _10xPV.Services;
using _10xPV.Services.ClimateImport;
using _10xPV.Services.Csv.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _10xPV.Controllers;

public class ClimateImportController : Controller
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private readonly IClimateImportOrchestrator _climateImportOrchestrator;
    private readonly IClimateDataResetService _resetService;
    private readonly ILogger<ClimateImportController> _logger;

    public ClimateImportController(IClimateImportOrchestrator climateImportOrchestrator, IClimateDataResetService resetService, ILogger<ClimateImportController> logger)
    {
        _climateImportOrchestrator = climateImportOrchestrator;
        _resetService = resetService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new ClimateImportPageViewModel
        {
            Form = new ClimateImportFormViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ClimateImportFormViewModel form)
    {
        ValidateForm(form);

        var pageViewModel = new ClimateImportPageViewModel
        {
            Form = form
        };

        if (!ModelState.IsValid)
        {
            return View(pageViewModel);
        }

        try
        {
            using var csvStream = form.File!.OpenReadStream();
            var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
            var importResult = await _climateImportOrchestrator.ImportAsync(
                csvStream,
                form.SchemaType!.Value,
                cancellationToken);

            pageViewModel = new ClimateImportPageViewModel
            {
                Form = form,
                Summary = new ClimateImportSummaryViewModel
                {
                    TotalRows = importResult.TotalRows,
                    ValidRows = importResult.ValidRows,
                    InvalidRows = importResult.InvalidRows,
                    StatusMessage = BuildStatusMessage(importResult)
                },
                Errors = importResult.Errors
                    .Select(MapError)
                    .ToArray()
            };

            return View(pageViewModel);
        }
        catch (OperationCanceledException)
        {
            ModelState.AddModelError(string.Empty, "Import anulowany.");
            return View(pageViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nieoczekiwany błąd podczas importu CSV.");
            ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas importu. Spróbuj ponownie.");
            return View(pageViewModel);
        }
    }

    private static ClimateImportErrorViewModel MapError(CsvRowError error)
    {
        return new ClimateImportErrorViewModel
        {
            LineNumber = error.LineNumber,
            Field = error.Field,
            Code = error.Code,
            Message = error.Message
        };
    }

    private static string BuildStatusMessage(CsvImportResult importResult)
    {
        var hasErrors = importResult.Errors.Count > 0;

        if (importResult.InvalidRows == 0 && !hasErrors)
        {
            return "Import zakończony sukcesem.";
        }

        if (importResult.ValidRows == 0)
        {
            return "Import zakończony z błędami.";
        }

        return "Import zakończony częściowym sukcesem.";
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetAllData()
    {
        try
        {
            await _resetService.DeleteAllAsync();
            TempData["SuccessMessage"] = "Wszystkie dane klimatyczne zostały usunięte.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania danych klimatycznych.");
            TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania danych. Spróbuj ponownie.";
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidateForm(ClimateImportFormViewModel form)
    {
        if (form.File is null)
        {
            ModelState.AddModelError(nameof(ClimateImportFormViewModel.File), "Plik CSV jest wymagany.");
            return;
        }

        if (form.File.Length == 0)
        {
            ModelState.AddModelError(nameof(ClimateImportFormViewModel.File), "Przesłany plik jest pusty.");
        }

        if (form.File.Length > MaxFileSizeBytes)
        {
            ModelState.AddModelError(nameof(ClimateImportFormViewModel.File), "Maksymalny rozmiar pliku to 10 MB.");
        }

        var extension = Path.GetExtension(form.File.FileName);
        if (!string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(ClimateImportFormViewModel.File), "Dozwolone są wyłącznie pliki z rozszerzeniem .csv.");
        }

        if (!form.SchemaType.HasValue)
        {
            ModelState.AddModelError(nameof(ClimateImportFormViewModel.SchemaType), "Wybierz typ schematu importu.");
        }
    }
}