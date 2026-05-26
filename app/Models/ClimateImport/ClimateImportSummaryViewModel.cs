namespace _10xPV.Models.ClimateImport;

public class ClimateImportSummaryViewModel
{
    public int TotalRows { get; init; }

    public int ValidRows { get; init; }

    public int InvalidRows { get; init; }

    public string? StatusMessage { get; init; }
}