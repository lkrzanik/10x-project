namespace _10xPV.Services.Csv.Contracts;

public sealed record CsvRowError(
    int LineNumber,
    string? Field,
    string Code,
    string Message);