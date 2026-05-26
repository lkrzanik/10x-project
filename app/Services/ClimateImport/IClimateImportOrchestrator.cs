using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Services.ClimateImport;

public interface IClimateImportOrchestrator
{
    Task<CsvImportResult> ImportAsync(
        Stream csvStream,
        CsvSchemaType schemaType,
        CancellationToken cancellationToken = default);
}
