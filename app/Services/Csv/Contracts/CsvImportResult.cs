namespace _10xPV.Services.Csv.Contracts;

public sealed record CsvImportResult(
    CsvSchemaType SchemaType,
    IReadOnlyList<SensorCsvRow> SensorRows,
    IReadOnlyList<WeatherCsvRow> WeatherRows,
    IReadOnlyList<CsvRowError> Errors,
    int TotalRows,
    int ValidRows,
    int InvalidRows)
{
    public static CsvImportResult Empty(CsvSchemaType schemaType) =>
        new(schemaType, [], [], [], 0, 0, 0);
}