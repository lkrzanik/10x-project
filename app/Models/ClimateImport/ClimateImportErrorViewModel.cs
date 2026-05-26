namespace _10xPV.Models.ClimateImport;

public class ClimateImportErrorViewModel
{
    public int LineNumber { get; init; }

    public string? Field { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}