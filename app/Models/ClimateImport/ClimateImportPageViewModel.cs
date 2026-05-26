namespace _10xPV.Models.ClimateImport;

public class ClimateImportPageViewModel
{
    public ClimateImportFormViewModel Form { get; init; } = new();

    public ClimateImportSummaryViewModel? Summary { get; init; }

    public IReadOnlyList<ClimateImportErrorViewModel> Errors { get; init; } = [];
}