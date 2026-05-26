using _10xPV.Models.ClimateImport;
using Microsoft.AspNetCore.Mvc;

namespace _10xPV.Controllers;

public class ClimateImportController : Controller
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

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
    public IActionResult Index(ClimateImportFormViewModel form)
    {
        ValidateForm(form);

        var viewModel = new ClimateImportPageViewModel
        {
            Form = form
        };

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        TempData["ImportNotImplemented"] = "Integracja parsera CSV zostanie dodana w fazie 2.";
        return View(viewModel);
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